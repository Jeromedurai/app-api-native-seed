namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for forgot password response
    /// </summary>
    public class ForgotPasswordResponse
    {
        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// OTP expiration time in seconds
        /// </summary>
        public int ExpiresInSeconds { get; set; }

        /// <summary>
        /// User ID (for internal use)
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// OTP (only returned in development/testing)
        /// </summary>
        public string OTP { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User first name (for email personalization)
        /// </summary>
        public string FirstName { get; set; }
    }
}
