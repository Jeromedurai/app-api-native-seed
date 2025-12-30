using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Payload for product search endpoint
    /// </summary>
    public class ProductSearchPayload
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
        /// Search in product name, description, code
        /// </summary>
        public string Search { get; set; } = string.Empty;

        /// <summary>
        /// Filter by category ID
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// Minimum price filter
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum price filter
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Minimum rating filter
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Filter by stock availability
        /// </summary>
        public bool? InStock { get; set; }

        /// <summary>
        /// Filter best sellers only
        /// </summary>
        public bool? BestSeller { get; set; }

        /// <summary>
        /// Filter products with offers
        /// </summary>
        public bool? HasOffer { get; set; }

        /// <summary>
        /// Sort field (productName, price, rating, userBuyCount, created)
        /// </summary>
        public string SortBy { get; set; } = "created";

        /// <summary>
        /// Sort direction (asc, desc)
        /// </summary>
        public string SortOrder { get; set; } = "desc";
    }
}
