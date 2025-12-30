using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.ProductCart
{
    /// <summary>
    /// Model for clearing entire cart request
    /// </summary>
    public class ClearCartRequest
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

    /// <summary>
    /// Model for clear cart response
    /// </summary>
    public class ClearCartResponse
    {
        /// <summary>
        /// User ID whose cart was cleared
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Number of unique items that were cleared
        /// </summary>
        public int ClearedItemCount { get; set; }

        /// <summary>
        /// Total quantity of all items that were cleared
        /// </summary>
        public int ClearedQuantity { get; set; }

        /// <summary>
        /// Total monetary value of cleared items
        /// </summary>
        public decimal ClearedValue { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the cart was cleared
        /// </summary>
        public DateTime ClearedDate { get; set; }

        /// <summary>
        /// Whether items were permanently deleted (true) or soft deleted (false)
        /// </summary>
        public bool WasHardDelete { get; set; }

        /// <summary>
        /// Updated cart summary after clearing (should be empty)
        /// </summary>
        public CartSummaryInfo Summary { get; set; } = new CartSummaryInfo();

        /// <summary>
        /// Popular products suggested for rebuilding the cart
        /// </summary>
        public List<PopularProduct> PopularProducts { get; set; } = new List<PopularProduct>();
    }

    /// <summary>
    /// Model for popular product suggestions after cart clearing
    /// </summary>
    public class PopularProduct
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Product rating
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Whether product is a best seller
        /// </summary>
        public bool BestSeller { get; set; }

        /// <summary>
        /// Whether product is trending
        /// </summary>
        public bool Trending { get; set; }

        /// <summary>
        /// Number of times this product was bought by users
        /// </summary>
        public int UserBuyCount { get; set; }

        /// <summary>
        /// Product offer/discount information
        /// </summary>
        public string Offer { get; set; }

        /// <summary>
        /// Main product image URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Product category name
        /// </summary>
        public string CategoryName { get; set; }
    }
}
