using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.ProductCart
{
    /// <summary>
    /// Model for getting user cart request
    /// </summary>
    public class GetCartRequest
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
        /// Session ID (optional for guest cart support)
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Include recommended products (default: true)
        /// </summary>
        public bool IncludeRecommendations { get; set; } = true;

        /// <summary>
        /// Include product images (default: true)
        /// </summary>
        public bool IncludeImages { get; set; } = true;

        /// <summary>
        /// Include category details (default: true)
        /// </summary>
        public bool IncludeCategoryDetails { get; set; } = true;

        /// <summary>
        /// Base URL for generating full image URLs (optional)
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
