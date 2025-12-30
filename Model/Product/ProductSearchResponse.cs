using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Response model for product search
    /// </summary>
    public class ProductSearchResponse
    {
        /// <summary>
        /// List of products
        /// </summary>
        public List<ProductDetailItem> Products { get; set; } = new List<ProductDetailItem>();

        /// <summary>
        /// Pagination information
        /// </summary>
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    /// <summary>
    /// Detailed product item for search results
    /// </summary>
    public class ProductDetailItem
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
        public int Category { get; set; }
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
        public List<ProductSearchImageInfo> Images { get; set; } = new List<ProductSearchImageInfo>();
    }

    /// <summary>
    /// Product image information for search results
    /// </summary>
    public class ProductSearchImageInfo
    {
        public long ImageId { get; set; }
        public long ProductId { get; set; }
        public string Poster { get; set; } = string.Empty;
        
        /// <summary>
        /// Full image URL
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;
        
        public bool Main { get; set; }
        public bool Active { get; set; }
        public int OrderBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    /// <summary>
    /// Pagination information
    /// </summary>
    public class PaginationInfo
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
