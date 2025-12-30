using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Request model for filtered product list endpoint
    /// </summary>
    public class ProductListRequest
    {
        /// <summary>
        /// Filter by category ID (optional)
        /// </summary>
        public long? CategoryId { get; set; }

        /// <summary>
        /// Filter by active status (optional - defaults to true if not specified)
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Page number for pagination (default: 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 10, max: 100)
        /// </summary>
        [Range(1, 1001, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Sort field (ProductName, Price, Rating, Category, Quantity, Created)
        /// </summary>
        public string SortBy { get; set; } = "Created";

        /// <summary>
        /// Sort direction (ASC, DESC)
        /// </summary>
        public string SortOrder { get; set; } = "DESC";
    }
}
