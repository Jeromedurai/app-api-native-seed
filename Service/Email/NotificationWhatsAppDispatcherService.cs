using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tenant.Query.Model.Email;
using Tenant.Query.Repository.Email;
using Tenant.Query.Repository.Product;
using Tenant.Query.Service.Whatsapp;

namespace Tenant.Query.Service.Email
{
    /// <summary>
    /// WhatsApp counterpart of <see cref="NotificationDispatcherService"/>: resolves a
    /// SA_EMAILMASTER row (TEMPLATEID + EMAILSP) into a WhatsApp template send.
    ///
    /// Gated by the feature flag "Notifications:WhatsApp:Enabled" (default false): when
    /// disabled it returns Skipped=true so the worker marks the queue row done (no send).
    /// The template's Meta-approved name comes from SA_EMAILTEMPLATE.WHATSAPPTEMPLATE; the
    /// recipient phone + merge fields come from the EMAILSP JSON payload.
    /// </summary>
    public class NotificationWhatsAppDispatcherService
    {
        private readonly EmailNotificationRepository _repository;
        private readonly IWhatsAppService _whatsApp;
        private readonly IConfiguration _configuration;
        private readonly ProductRepository _productRepository;
        private readonly ILogger<NotificationWhatsAppDispatcherService> _logger;

        public NotificationWhatsAppDispatcherService(
            EmailNotificationRepository repository,
            IWhatsAppService whatsApp,
            IConfiguration configuration,
            ProductRepository productRepository,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _whatsApp = whatsApp;
            _configuration = configuration;
            _productRepository = productRepository;
            _logger = loggerFactory.CreateLogger<NotificationWhatsAppDispatcherService>();
        }

        public async Task<SendEmailResponse> DispatchAsync(QueuedEmailDispatchRequest request)
        {
            if (request == null)
                return Failure("Dispatch request is required.");

            // Feature flag — disabled means "done, don't retry, no send".
            if (!IsEnabled())
            {
                _logger.LogInformation("WhatsApp channel disabled — skipping queue Id {QueueId}.", request.Id);
                return Skipped("WhatsApp channel is disabled.");
            }

            var template = await _repository.GetTemplateByIdAsync(request.TemplateId, request.TenantId);
            if (template == null)
                return Failure($"TemplateId {request.TemplateId} not found.");
            if (!template.Active)
                return Skipped($"Template '{template.TemplateName}' is inactive.");

            if (string.IsNullOrWhiteSpace(template.WhatsAppTemplate))
            {
                _logger.LogInformation(
                    "Template {TemplateId} ({Name}) has no WhatsApp template configured — skipping.",
                    template.TemplateId, template.TemplateName);
                return Skipped("No WhatsApp template configured for this template.");
            }

            var data = ParseJsonPayload(request.EmailSP);
            var phone = AsString(data, "Phone") ?? AsString(data, "Mobile") ?? AsString(data, "To");
            if (string.IsNullOrWhiteSpace(phone))
                return Failure("Recipient phone is missing from EMAILSP payload.");

            try
            {
                var ok = await _whatsApp.SendTemplateAsync(phone, template.WhatsAppTemplate, data);
                return ok
                    ? new SendEmailResponse { Success = true, To = phone, Subject = template.TemplateName, Message = "WhatsApp sent." }
                    : Failure("WhatsApp provider rejected the message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WhatsApp dispatch failed for queue Id {QueueId}.", request.Id);
                return Failure($"WhatsApp send error: {ex.Message}");
            }
        }

        // AppSettings (WHATSAPP_ENABLED) → appsettings.json (Notifications:WhatsApp:Enabled) → default false.
        private bool IsEnabled()
        {
            var jsonOn = bool.TryParse(_configuration["Notifications:WhatsApp:Enabled"], out var on) && on;
            return _productRepository.GetSettingBool("WHATSAPP_ENABLED", jsonOn);
        }

        private static IDictionary<string, object> ParseJsonPayload(string emailSp)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(emailSp)) return dict;
            var trimmed = emailSp.TrimStart();
            if (trimmed.Length == 0 || trimmed[0] != '{') return dict;
            try
            {
                using var doc = JsonDocument.Parse(emailSp);
                if (doc.RootElement.ValueKind != JsonValueKind.Object) return dict;
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    dict[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.ToString();
                }
            }
            catch (JsonException) { /* non-JSON payload → empty */ }
            return dict;
        }

        private static string AsString(IDictionary<string, object> data, string key)
        {
            if (data == null || !data.TryGetValue(key, out var v) || v == null) return null;
            var s = v.ToString();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        private static SendEmailResponse Failure(string message) =>
            new SendEmailResponse { Success = false, Message = message, To = string.Empty, Subject = string.Empty };

        private static SendEmailResponse Skipped(string reason) =>
            new SendEmailResponse { Success = true, Skipped = true, Message = reason, To = string.Empty, Subject = string.Empty };
    }
}
