using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tenant.Query.Service.Sms
{
    /// <summary>
    /// Development stub: logs OTP to server output until MSG91 (or another provider) is configured.
    /// Replace registration in Startup.cs with Msg91SmsService when Sms:Provider = "Msg91".
    /// </summary>
    public class LoggingSmsService : ISmsService
    {
        private readonly ILogger<LoggingSmsService> _logger;

        public LoggingSmsService(ILogger<LoggingSmsService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendLoginOtpAsync(string phone, string otp, int expiresInSeconds, string firstName = null)
        {
            _logger.LogInformation(
                "SMS stub — Login OTP for {Phone} ({FirstName}): {Otp} (expires in {Seconds}s). Configure ISmsService for production.",
                phone,
                firstName ?? "User",
                otp,
                expiresInSeconds);

            return Task.FromResult(true);
        }
    }
}
