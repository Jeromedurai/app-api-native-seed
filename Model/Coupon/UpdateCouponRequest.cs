using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for updating a coupon request
    /// </summary>
    public class UpdateCouponRequest
    {
        /// <summary>
        /// Coupon ID (required)
        /// </summary>
        [Required(ErrorMessage = "Coupon ID is required")]
        public long CouponId { get; set; }

        /// <summary>
        /// Coupon code (optional)
        /// </summary>
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        public string Code { get; set; }

        /// <summary>
        /// Coupon type: 'percentage' or 'fixed' (optional)
        /// </summary>
        [RegularExpression("^(percentage|fixed)$", ErrorMessage = "Coupon type must be either 'percentage' or 'fixed'")]
        public string Type { get; set; }

        /// <summary>
        /// Discount value (optional)
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        public decimal? Value { get; set; }

        /// <summary>
        /// Minimum order amount (optional)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Minimum amount must be 0 or greater")]
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Maximum discount amount (optional)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount must be 0 or greater")]
        public decimal? MaxDiscount { get; set; }

        /// <summary>
        /// Coupon description (optional)
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Start date (optional)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date (optional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Usage limit (optional)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be greater than 0")]
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Per-user usage limit (optional)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Per-user usage limit must be greater than 0")]
        public int? UsageLimitPerUser { get; set; }

        /// <summary>
        /// Active status (optional)
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Tenant ID (required)
        /// </summary>
        [Required(ErrorMessage = "Tenant ID is required")]
        public long TenantId { get; set; }
    }
}

