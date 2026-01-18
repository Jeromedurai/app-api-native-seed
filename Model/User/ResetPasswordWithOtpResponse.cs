using System;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for reset password with OTP response
    /// </summary>
    public class ResetPasswordWithOtpResponse
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Password reset timestamp
        /// </summary>
        public DateTime ResetAt { get; set; }
    }
}
