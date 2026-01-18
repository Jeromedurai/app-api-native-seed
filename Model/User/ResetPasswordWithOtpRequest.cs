using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for reset password with OTP request
    /// </summary>
    public class ResetPasswordWithOtpRequest
    {
        /// <summary>
        /// User email address (required)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        /// <summary>
        /// OTP code (required)
        /// </summary>
        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string OTP { get; set; }

        /// <summary>
        /// New password (required)
        /// </summary>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirm password (required)
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
