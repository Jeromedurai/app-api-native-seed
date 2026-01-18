using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for forgot password request
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// User email address (required)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }
    }
}
