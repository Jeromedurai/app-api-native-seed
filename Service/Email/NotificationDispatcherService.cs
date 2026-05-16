using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tenant.Query.Model.Email;
using Tenant.Query.Repository.Email;

namespace Tenant.Query.Service.Email
{
    /// <summary>
    /// Resolves a SA_EMAILMASTER queue row (TEMPLATEID + EMAILSP) into a SendEmailRequest and
    /// hands it to the existing EmailService for Razor render + SMTP delivery.
    ///
    /// EMAILSP convention (in priority order):
    ///   1. Valid JSON object  → keys map directly into TemplateData (case-insensitive).
    ///                           "to" (or "email"), "subject", "name", "body" populate
    ///                           the SendEmailRequest envelope.
    ///                           Example: {"to":"a@b.com","subject":"Hi","orderId":42,"items":[…]}
    ///   2. Contains '|'       → legacy dynamic form "SP_NAME|param1,param2".
    ///                           Runs the SP, projects first row into TemplateData.
    ///   3. Otherwise          → legacy static form "Email~Name~Body".
    ///
    /// Templates are cached for 5 seconds (sliding window) since they change rarely
    /// and the worker may dispatch hundreds of rows per cycle.
    ///
    /// Inactive-template behavior: returns { Success=true, Skipped=true } so the
    /// worker treats the row as "done" and stops retrying. The worker's row was
    /// already filtered out by SA_GetEmailsWaitingToBeSend.JOIN ACTIVE=1 — this
    /// path only triggers for retry rows whose template was active when enqueued
    /// but has since been deactivated.
    /// </summary>
    public class NotificationDispatcherService
    {
        private const string DefaultViewName = "GenericTextEmail";
        // Cache key includes tenant id so a tenant override doesn't shadow the
        // global row for a different tenant in the same process.
        private const string TemplateCacheKeyPrefix = "EmailTemplate:";
        private static readonly TimeSpan TemplateCacheWindow = TimeSpan.FromSeconds(5);

        private readonly EmailNotificationRepository _repository;
        private readonly EmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<NotificationDispatcherService> _logger;

        public NotificationDispatcherService(
            EmailNotificationRepository repository,
            EmailService emailService,
            IMemoryCache cache,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _emailService = emailService;
            _cache = cache;
            _logger = loggerFactory.CreateLogger<NotificationDispatcherService>();
        }

        public async Task<SendEmailResponse> DispatchAsync(QueuedEmailDispatchRequest request)
        {
            if (request == null)
            {
                return Failure(null, null, "Dispatch request is required.");
            }

            // Structured log fields for ops dashboards (#11) — tenant id included.
            using var _ = _logger.BeginScope(new Dictionary<string, object>
            {
                ["QueueId"]       = request.Id,
                ["TenantId"]      = request.TenantId,
                ["TemplateId"]    = request.TemplateId,
                ["RetryCount"]    = request.RetryCount,
                ["CorrelationId"] = request.CorrelationId ?? string.Empty
            });

            if (request.RetryCount > 0)
            {
                _logger.LogInformation("Dispatching retry attempt {RetryCount} for queue Id {QueueId} (tenant {TenantId})",
                    request.RetryCount, request.Id, request.TenantId);
            }

            var template = await GetTemplateCachedAsync(request.TemplateId, request.TenantId);
            if (template == null)
            {
                return Failure(null, null, $"TemplateId {request.TemplateId} not found.");
            }
            if (!template.Active)
            {
                _logger.LogInformation(
                    "Template {TemplateId} ({TemplateName}) is inactive; marking queue Id {QueueId} as skipped.",
                    template.TemplateId, template.TemplateName, request.Id);
                return Skipped(template.TemplateName, $"Template '{template.TemplateName}' is inactive.");
            }

            string subject = template.TemplateName;
            IDictionary<string, object> templateData;

            try
            {
                templateData = await ParseEmailSpAsync(request.EmailSP);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse/enrich EMAILSP for queue Id {QueueId}.", request.Id);
                return Failure(null, subject, $"Failed to parse EMAILSP: {ex.Message}");
            }

            // Resolve envelope fields from the parsed payload (case-insensitive lookup).
            var to       = AsString(templateData, "To") ?? AsString(templateData, "Email");
            var name     = AsString(templateData, "Name") ?? AsString(templateData, "FirstName");
            var body     = AsString(templateData, "Body") ?? AsString(templateData, "Message");
            var spSubject = AsString(templateData, "Subject");
            if (!string.IsNullOrWhiteSpace(spSubject)) subject = spSubject;

            if (string.IsNullOrWhiteSpace(to))
            {
                return Failure(null, subject, "Recipient (To/Email) is missing from EMAILSP payload.");
            }

            // Normalize standard keys for the Razor view.
            templateData["To"]            = to;
            templateData["Name"]          = name ?? string.Empty;
            templateData["Subject"]       = subject;
            templateData["Body"]          = body ?? string.Empty;
            templateData["TemplateName"]  = template.TemplateName;
            templateData["CorrelationId"] = request.CorrelationId ?? string.Empty;
            templateData["TenantId"]      = request.TenantId;

            var sendRequest = new SendEmailRequest
            {
                To = to,
                Subject = subject,
                TemplateName = !string.IsNullOrWhiteSpace(template.ViewName) ? template.ViewName : DefaultViewName,
                TemplateData = new Dictionary<string, object>(templateData, StringComparer.OrdinalIgnoreCase)
            };

            return await _emailService.SendEmail(sendRequest);
        }

        // ---- Template cache ----------------------------------------------------

        /// <summary>
        /// Cache key is (templateId, tenantId) so a tenant-specific override and
        /// the global row coexist independently in the cache.
        /// </summary>
        private async Task<EmailTemplateDto> GetTemplateCachedAsync(int templateId, long tenantId)
        {
            var key = $"{TemplateCacheKeyPrefix}{templateId}:{tenantId}";
            if (_cache.TryGetValue(key, out EmailTemplateDto cached))
            {
                return cached;
            }

            var template = await _repository.GetTemplateByIdAsync(templateId, tenantId);
            if (template != null)
            {
                _cache.Set(key, template, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TemplateCacheWindow
                });
            }
            return template;
        }

        // ---- EMAILSP parsing ---------------------------------------------------

        /// <summary>
        /// Parse EMAILSP using the priority list documented at the top of this class.
        /// Always returns a case-insensitive dictionary (never null).
        /// </summary>
        private async Task<IDictionary<string, object>> ParseEmailSpAsync(string emailSp)
        {
            if (string.IsNullOrWhiteSpace(emailSp))
            {
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            // Priority 1: JSON object — most flexible, recommended for new callers.
            var trimmed = emailSp.TrimStart();
            if (trimmed.Length > 0 && trimmed[0] == '{')
            {
                try
                {
                    using var doc = JsonDocument.Parse(emailSp);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        return JsonObjectToDictionary(doc.RootElement);
                    }
                }
                catch (JsonException)
                {
                    // Fall through to legacy parsers.
                }
            }

            // Priority 2: legacy dynamic "SP_NAME|p1,p2".
            if (emailSp.Contains("|"))
            {
                return await BuildFromDynamicSpAsync(emailSp);
            }

            // Priority 3: legacy static "Email~Name~Body".
            return BuildFromStaticPayload(emailSp);
        }

        private static IDictionary<string, object> JsonObjectToDictionary(JsonElement obj)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in obj.EnumerateObject())
            {
                dict[prop.Name] = JsonValueToClr(prop.Value);
            }
            return dict;
        }

        private static object JsonValueToClr(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.String: return value.GetString();
                case JsonValueKind.Number:
                    if (value.TryGetInt64(out var l)) return l;
                    if (value.TryGetDouble(out var d)) return d;
                    return value.GetRawText();
                case JsonValueKind.True:   return true;
                case JsonValueKind.False:  return false;
                case JsonValueKind.Null:   return null;
                case JsonValueKind.Array:
                    return value.EnumerateArray().Select(JsonValueToClr).ToList();
                case JsonValueKind.Object:
                    return JsonObjectToDictionary(value);
                default: return value.GetRawText();
            }
        }

        private async Task<IDictionary<string, object>> BuildFromDynamicSpAsync(string emailSp)
        {
            var pipe = emailSp.IndexOf('|');
            var spName = emailSp.Substring(0, pipe).Trim();
            var paramCsv = pipe + 1 < emailSp.Length ? emailSp.Substring(pipe + 1) : string.Empty;

            var parameters = string.IsNullOrWhiteSpace(paramCsv)
                ? Array.Empty<string>()
                : paramCsv.Split(new[] { ',' }, StringSplitOptions.None)
                          .Select(p => p?.Trim())
                          .ToArray();

            if (string.IsNullOrWhiteSpace(spName))
            {
                throw new ArgumentException("Dynamic EMAILSP is missing the SP name before '|'.");
            }

            var row = await _repository.ExecuteDynamicEmailSpAsync(spName, parameters);
            return row ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        private static IDictionary<string, object> BuildFromStaticPayload(string emailSp)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var parts = emailSp.Split(new[] { '~' }, 3);
            if (parts.Length > 0) dict["To"]   = parts[0]?.Trim();
            if (parts.Length > 1) dict["Name"] = parts[1]?.Trim();
            if (parts.Length > 2) dict["Body"] = parts[2];
            return dict;
        }

        private static string AsString(IDictionary<string, object> data, string key)
        {
            if (data == null || !data.TryGetValue(key, out var value) || value == null) return null;
            var s = value.ToString();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        private static SendEmailResponse Failure(string to, string subject, string message) =>
            new SendEmailResponse
            {
                Success = false,
                Message = message,
                To = to ?? string.Empty,
                Subject = subject ?? string.Empty
            };

        private static SendEmailResponse Skipped(string subject, string reason) =>
            new SendEmailResponse
            {
                Success = true,   // Worker treats 200 as success → no retry.
                Skipped = true,
                Message = reason,
                To = string.Empty,
                Subject = subject ?? string.Empty
            };
    }
}
