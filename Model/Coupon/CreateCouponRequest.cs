using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for creating a coupon request
    /// </summary>
    public class CreateCouponRequest
    {
        /// <summary>
        /// Coupon code (required, unique per tenant)
        /// </summary>
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        public string Code { get; set; }

        /// <summary>
        /// Coupon type: 'percentage' or 'fixed' (required)
        /// </summary>
        [Required(ErrorMessage = "Coupon type is required")]
        [RegularExpression("^(percentage|fixed)$", ErrorMessage = "Coupon type must be either 'percentage' or 'fixed'")]
        public string Type { get; set; }

        /// <summary>
        /// Discount value (required)
        /// For percentage: 0-100, For fixed: amount in currency
        /// </summary>
        [Required(ErrorMessage = "Discount value is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        public decimal Value { get; set; }

        /// <summary>
        /// Minimum order amount required to use this coupon (optional)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Minimum amount must be 0 or greater")]
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Maximum discount amount (required for percentage type, optional for fixed)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount must be 0 or greater")]
        public decimal? MaxDiscount { get; set; }

        /// <summary>
        /// Coupon description (optional)
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Start date for coupon validity (required)
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date for coupon validity (required)
        /// </summary>
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Maximum number of times this coupon can be used (optional)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be greater than 0")]
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Maximum number of times a single user can use this coupon (optional)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Per-user usage limit must be greater than 0")]
        public int? UsageLimitPerUser { get; set; }

        /// <summary>
        /// Whether the coupon is active (default: true)
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Tenant ID (required)
        /// </summary>
        [Required(ErrorMessage = "Tenant ID is required")]
        public long TenantId { get; set; }
    }
}

