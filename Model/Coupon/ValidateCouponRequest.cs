using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for validating/checking a coupon request
    /// </summary>
    public class ValidateCouponRequest
    {
        /// <summary>
        /// Coupon code to validate (required)
        /// </summary>
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50, ErrorMessage = "Coupon code cannot exceed 50 characters")]
        public string Code { get; set; }

        /// <summary>
        /// Order amount (required)
        /// </summary>
        [Required(ErrorMessage = "Order amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Order amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// User ID (optional, for per-user usage limit checking)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (required)
        /// </summary>
        [Required(ErrorMessage = "Tenant ID is required")]
        public long TenantId { get; set; }
    }
}

