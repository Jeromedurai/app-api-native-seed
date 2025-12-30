using System;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Response model for product review
    /// </summary>
    public class ProductReviewResponse
    {
        /// <summary>
        /// Review ID
        /// </summary>
        public long ReviewId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Rating (1-5)
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Review comment/text
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Whether this is a verified purchase
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// Helpful count
        /// </summary>
        public int Helpful { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Updated date
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}

