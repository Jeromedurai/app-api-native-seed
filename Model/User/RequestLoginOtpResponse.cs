namespace Tenant.Query.Model.User
{
    public class RequestLoginOtpResponse
    {
        public string Message { get; set; }
        public int ExpiresInSeconds { get; set; }
        public long UserId { get; set; }
        public string OTP { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
    }
}
