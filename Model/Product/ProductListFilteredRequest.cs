using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Request model for filtered product list
    /// </summary>
    public class ProductListFilteredRequest
    {
        /// <summary>
        /// Page number (default: 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Items per page (default: 10)
        /// </summary>
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Sort field (productName, price, rating, userBuyCount, created)
        /// </summary>
        public string SortBy { get; set; } = "productName";

        /// <summary>
        /// Sort direction (ASC, DESC)
        /// </summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>
        /// Filter by category ID (optional, empty string means all categories)
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Filter by active status (optional)
        /// </summary>
        public bool? Active { get; set; }
    }
}

