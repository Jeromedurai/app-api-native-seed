using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.WishList
{
    /// <summary>
    /// Model for clearing entire wishlist request
    /// </summary>
    public class ClearWishlistRequest
    {
        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Whether to clear completely (true) or mark inactive (false)
        /// Default: true (permanent removal)
        /// </summary>
        public bool ClearCompletely { get; set; } = true;

        /// <summary>
        /// IP address of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string UserAgent { get; set; }
    }
}
