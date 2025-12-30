using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user login request
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User email or phone number (required)
        /// </summary>
        [Required(ErrorMessage = "Email or phone number is required")]
        [StringLength(255, ErrorMessage = "Email or phone number cannot exceed 255 characters")]
        public string EmailOrPhone { get; set; }

        /// <summary>
        /// User password (required)
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters")]
        public string Password { get; set; }

        /// <summary>
        /// Remember me option (default: false)
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }
}
