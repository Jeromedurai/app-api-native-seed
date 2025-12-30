using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Request model for creating a product review
    /// </summary>
    public class CreateProductReviewRequest
    {
        /// <summary>
        /// Product ID (required)
        /// </summary>
        [Required(ErrorMessage = "Product ID is required")]
        public long ProductId { get; set; }

        /// <summary>
        /// User ID (optional - will be taken from JWT token if not provided)
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional - will be taken from JWT token if not provided)
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Rating (1-5) (required)
        /// </summary>
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Review title (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Review title cannot exceed 255 characters")]
        public string ReviewTitle { get; set; }

        /// <summary>
        /// Review text/comment (optional)
        /// </summary>
        public string ReviewText { get; set; }
    }
}

