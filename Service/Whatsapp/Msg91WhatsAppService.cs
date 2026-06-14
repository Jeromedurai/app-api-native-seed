using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tenant.Query.Service.Whatsapp
{
    /// <summary>
    /// Sends WhatsApp messages via MSG91's WhatsApp API using a pre-approved template.
    /// SKELETON: the request shape below follows MSG91's WhatsApp template send; verify
    /// the exact endpoint/body against your MSG91 WhatsApp account before going live.
    /// Configure WhatsApp:Provider = "Msg91" and WhatsApp:Msg91:* (falls back to Sms:Msg91:AuthKey).
    /// </summary>
    public class Msg91WhatsAppService : IWhatsAppService
    {
        // MSG91 WhatsApp send endpoint (confirm for your account/integrated number).
        private const string WhatsAppApiUrl = "https://control.msg91.com/api/v5/whatsapp/whatsapp-outbound-message/";
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly ILogger<Msg91WhatsAppService> _logger;
        private readonly IConfiguration _configuration;

        public Msg91WhatsAppService(ILogger<Msg91WhatsAppService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendTemplateAsync(string toPhone, string templateName, IDictionary<string, object> data)
        {
            var authKey = _configuration["WhatsApp:Msg91:AuthKey"] ?? _configuration["Sms:Msg91:AuthKey"];
            var integratedNumber = _configuration["WhatsApp:Msg91:IntegratedNumber"];
            var nameSpace = _configuration["WhatsApp:Msg91:TemplateNamespace"];
            var langCode = _configuration["WhatsApp:Msg91:LanguageCode"] ?? "en";

            if (string.IsNullOrWhiteSpace(authKey) || string.IsNullOrWhiteSpace(templateName))
            {
                _logger.LogError(
                    "MSG91 WhatsApp not configured (AuthKey/template missing). Cannot send to {Phone}.", toPhone);
                return false;
            }

            var mobile = NormalizeMobile(toPhone);
            if (string.IsNullOrWhiteSpace(mobile))
            {
                _logger.LogWarning("WhatsApp recipient has no phone number; skipped.");
                return false;
            }

            // Body variables passed in declared order (Name, then CouponCode if present).
            var components = BuildBodyComponents(data);

            var payload = new Dictionary<string, object>
            {
                ["integrated_number"] = integratedNumber,
                ["content_type"] = "template",
                ["payload"] = new Dictionary<string, object>
                {
                    ["messaging_product"] = "whatsapp",
                    ["type"] = "template",
                    ["template"] = new Dictionary<string, object>
                    {
                        ["name"] = templateName,
                        ["namespace"] = nameSpace,
                        ["language"] = new Dictionary<string, object> { ["code"] = langCode, ["policy"] = "deterministic" },
                        ["to_and_components"] = new[]
                        {
                            new Dictionary<string, object>
                            {
                                ["to"] = new[] { mobile },
                                ["components"] = components
                            }
                        }
                    }
                }
            };

            try
            {
                var json = JsonSerializer.Serialize(payload);
                using var request = new HttpRequestMessage(HttpMethod.Post, WhatsAppApiUrl);
                request.Headers.TryAddWithoutValidation("authkey", authKey);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await HttpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("MSG91 WhatsApp failed for {Phone}. Status: {Status}, Body: {Body}",
                        toPhone, (int)response.StatusCode, body);
                    return false;
                }

                _logger.LogInformation("MSG91 WhatsApp template '{Template}' sent to {Phone}", templateName, toPhone);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MSG91 WhatsApp error for {Phone}: {Message}", toPhone, ex.Message);
                return false;
            }
        }

        /// <summary>Maps merge fields to a WhatsApp "body" component with ordered text parameters.</summary>
        private static object BuildBodyComponents(IDictionary<string, object> data)
        {
            var orderedKeys = new[] { "Name", "CouponCode" };
            var parameters = orderedKeys
                .Where(k => data != null && data.TryGetValue(k, out var v) && v != null && !string.IsNullOrWhiteSpace(v.ToString()))
                .Select(k => new Dictionary<string, object> { ["type"] = "text", ["text"] = data[k].ToString() })
                .ToList();

            return new[]
            {
                new Dictionary<string, object> { ["type"] = "body", ["parameters"] = parameters }
            };
        }

        private static string NormalizeMobile(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return phone;
            var digits = phone.Trim().Replace("+", "").Replace("-", "").Replace(" ", "");
            if (digits.Length == 10) return "91" + digits;
            if (digits.StartsWith("91") && digits.Length == 12) return digits;
            return digits;
        }
    }
}
