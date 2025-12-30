using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for getting coupons list request
    /// </summary>
    public class GetCouponsRequest
    {
        /// <summary>
        /// Tenant ID (required)
        /// </summary>
        [Required(ErrorMessage = "Tenant ID is required")]
        public long TenantId { get; set; }

        /// <summary>
        /// Page number (default: 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 10)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Status filter: 'active', 'inactive', 'expired', or 'all' (optional)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Search term for code or description (optional)
        /// </summary>
        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string Search { get; set; }
    }
}

