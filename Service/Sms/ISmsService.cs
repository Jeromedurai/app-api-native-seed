using System.Threading.Tasks;

namespace Tenant.Query.Service.Sms
{
    /// <summary>
    /// Sends transactional SMS messages (OTP delivery).
    /// Phase 1: LoggingSmsService logs OTPs for development.
    /// Phase 2: Set Sms:Provider to "Msg91" and configure Sms:Msg91:AuthKey, TemplateId, OtpVariable (DLT).
    /// </summary>
    public interface ISmsService
    {
        Task<bool> SendLoginOtpAsync(string phone, string otp, int expiresInSeconds, string firstName = null);
    }
}
