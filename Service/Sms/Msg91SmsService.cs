using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tenant.Query.Service.Sms
{
    /// <summary>
    /// Sends login OTP SMS via MSG91 Flow API (DLT template).
    /// Configure Sms:Provider = "Msg91" and Sms:Msg91:* in appsettings / user secrets.
    /// </summary>
    public class Msg91SmsService : ISmsService
    {
        private const string FlowApiUrl = "https://control.msg91.com/api/v5/flow/";
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly ILogger<Msg91SmsService> _logger;
        private readonly IConfiguration _configuration;

        public Msg91SmsService(ILogger<Msg91SmsService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendLoginOtpAsync(string phone, string otp, int expiresInSeconds, string firstName = null)
        {
            var authKey = _configuration["Sms:Msg91:AuthKey"];
            var templateId = _configuration["Sms:Msg91:TemplateId"];
            var otpVarName = _configuration["Sms:Msg91:OtpVariable"] ?? "OTP";

            if (string.IsNullOrWhiteSpace(authKey) || string.IsNullOrWhiteSpace(templateId))
            {
                _logger.LogError(
                    "MSG91 not configured (AuthKey/TemplateId missing). Cannot send OTP to {Phone}.",
                    phone);
                return false;
            }

            var mobile = NormalizeMobile(phone);
            var recipient = new Dictionary<string, string>
            {
                ["mobiles"] = mobile,
                [otpVarName] = otp
            };

            var nameVar = _configuration["Sms:Msg91:NameVariable"];
            if (!string.IsNullOrWhiteSpace(nameVar) && !string.IsNullOrWhiteSpace(firstName))
            {
                recipient[nameVar] = firstName;
            }

            var payload = new Dictionary<string, object>
            {
                ["template_id"] = templateId,
                ["short_url"] = "0",
                ["recipients"] = new[] { recipient }
            };

            try
            {
                var json = JsonSerializer.Serialize(payload);
                using var request = new HttpRequestMessage(HttpMethod.Post, FlowApiUrl);
                request.Headers.TryAddWithoutValidation("authkey", authKey);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await HttpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "MSG91 SMS failed for {Phone}. Status: {Status}, Body: {Body}",
                        phone, (int)response.StatusCode, body);
                    return false;
                }

                _logger.LogInformation("MSG91 login OTP sent to {Phone}", phone);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MSG91 SMS error for {Phone}: {Message}", phone, ex.Message);
                return false;
            }
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
