using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.ProductCart
{
    /// <summary>
    /// Model for removing item from cart request
    /// </summary>
    public class RemoveFromCartRequest
    {
        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Product ID to remove (required)
        /// </summary>
        [Required(ErrorMessage = "Product ID is required")]
        public long ProductId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Whether to remove completely (true) or mark inactive (false)
        /// Default: true (permanent removal)
        /// </summary>
        public bool RemoveCompletely { get; set; } = true;

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
    /// Model for remove from cart response
    /// </summary>
    public class RemoveFromCartResponse
    {
        /// <summary>
        /// Cart ID that was removed
        /// </summary>
        public long CartId { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Product ID that was removed
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Name of the removed product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Quantity that was removed
        /// </summary>
        public int RemovedQuantity { get; set; }

        /// <summary>
        /// Price per unit of the removed product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total value of removed items (quantity * price)
        /// </summary>
        public decimal ItemTotal { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the item was removed
        /// </summary>
        public DateTime RemovedDate { get; set; }

        /// <summary>
        /// Updated cart summary after removal
        /// </summary>
        public CartSummaryInfo Summary { get; set; } = new CartSummaryInfo();

        /// <summary>
        /// Recommended products to replace the removed item
        /// </summary>
        public List<RecommendedProduct> RecommendedProducts { get; set; } = new List<RecommendedProduct>();
    }

    /// <summary>
    /// Model for recommended product to replace removed item
    /// </summary>
    public class RecommendedProduct
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
        /// Product offer/discount information
        /// </summary>
        public string Offer { get; set; }

        /// <summary>
        /// Main product image URL
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
