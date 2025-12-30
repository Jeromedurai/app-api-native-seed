using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Response model for filtered product list endpoint
    /// </summary>
    public class ProductListResponse
    {
        /// <summary>
        /// List of products
        /// </summary>
        public List<ProductListItem> Products { get; set; } = new List<ProductListItem>();

        /// <summary>
        /// Pagination information
        /// </summary>
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    /// <summary>
    /// Individual product item in the list
    /// </summary>
    public class ProductListItem
    {
        public long ProductId { get; set; }
        public long TenantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string FullDescription { get; set; } = string.Empty;
        public string Specification { get; set; } = string.Empty;
        public string Story { get; set; } = string.Empty;
        public int PackQuantity { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        public decimal Price { get; set; }
        public long Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public bool Active { get; set; }
        public int Trending { get; set; }
        public int UserBuyCount { get; set; }
        public int Return { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool InStock { get; set; }
        public bool BestSeller { get; set; }
        public int DeliveryDate { get; set; }
        public string Offer { get; set; } = string.Empty;
        public int OrderBy { get; set; }
        public long UserId { get; set; }
        public string Overview { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;

        /// <summary>
        /// Product images with URLs
        /// </summary>
        public List<ProductSearchImageInfo> Images { get; set; } = new List<ProductSearchImageInfo>();
    }

}
