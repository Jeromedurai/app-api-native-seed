using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user registration request
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// User's full name (required)
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 255 characters")]
        public string Name { get; set; }

        /// <summary>
        /// User's email address (required)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }

        /// <summary>
        /// User's phone number (required)
        /// </summary>
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
        public string Phone { get; set; }

        /// <summary>
        /// User's password (required)
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 255 characters")]
        public string Password { get; set; }

        /// <summary>
        /// Password confirmation (required)
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Agreement to terms and conditions (required)
        /// </summary>
        [Required(ErrorMessage = "You must agree to the terms and conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions")]
        public bool AgreeToTerms { get; set; }

        /// <summary>
        /// Tenant ID (optional, defaults to 1)
        /// </summary>
        public long TenantId { get; set; } = 1;
    }
}
