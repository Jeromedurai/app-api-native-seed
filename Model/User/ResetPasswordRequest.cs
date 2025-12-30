using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for password reset request
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// New password (required)
        /// </summary>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 255 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirm new password (required)
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// IP address of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for password reset response
    /// </summary>
    public class ResetPasswordResponse
    {
        /// <summary>
        /// User ID of the user whose password was reset
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the password was reset
        /// </summary>
        public DateTime ResetDate { get; set; }

        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; } = true;
    }
}
