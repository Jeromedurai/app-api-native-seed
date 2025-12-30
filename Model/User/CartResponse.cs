using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user cart response
    /// </summary>
    public class CartResponse
    {
        /// <summary>
        /// List of cart items
        /// </summary>
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        /// <summary>
        /// Cart summary information
        /// </summary>
        public CartSummary Summary { get; set; } = new CartSummary();

        /// <summary>
        /// Recommended products based on cart items
        /// </summary>
        public List<RecommendedProduct> RecommendedProducts { get; set; } = new List<RecommendedProduct>();
    }

    /// <summary>
    /// Model for individual cart item
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Unique cart item identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Cart ID
        /// </summary>
        public long CartId { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Quantity of the product in cart
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Date when item was added to cart
        /// </summary>
        public DateTime AddedDate { get; set; }

        /// <summary>
        /// Date when item was last updated
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Session ID when item was added
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Complete product information
        /// </summary>
        public CartProductDetails Product { get; set; } = new CartProductDetails();

        /// <summary>
        /// Total price for this cart item (quantity * product price)
        /// </summary>
        public decimal ItemTotal { get; set; }

        /// <summary>
        /// Whether the requested quantity is available in stock
        /// </summary>
        public bool IsAvailable { get; set; }
    }

    /// <summary>
    /// Model for product details in cart
    /// </summary>
    public class CartProductDetails
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
        /// Product description
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Product code/SKU
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Full product description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Product specifications
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// Product story
        /// </summary>
        public string Story { get; set; }

        /// <summary>
        /// Pack quantity
        /// </summary>
        public int PackQuantity { get; set; }

        /// <summary>
        /// Available quantity in stock
        /// </summary>
        public int AvailableQuantity { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Product rating
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Whether product is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Whether product is trending
        /// </summary>
        public bool Trending { get; set; }

        /// <summary>
        /// Number of times product was bought by users
        /// </summary>
        public int UserBuyCount { get; set; }

        /// <summary>
        /// Return policy (in days)
        /// </summary>
        public int ReturnPolicy { get; set; }

        /// <summary>
        /// Product creation date
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Product modification date
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Whether product is in stock
        /// </summary>
        public bool InStock { get; set; }

        /// <summary>
        /// Whether product is a best seller
        /// </summary>
        public bool BestSeller { get; set; }

        /// <summary>
        /// Delivery date (in days)
        /// </summary>
        public int DeliveryDate { get; set; }

        /// <summary>
        /// Product offer/discount information
        /// </summary>
        public string Offer { get; set; }

        /// <summary>
        /// Product overview
        /// </summary>
        public string Overview { get; set; }

        /// <summary>
        /// Long product description
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// Product category information
        /// </summary>
        public CartProductCategory Category { get; set; } = new CartProductCategory();

        /// <summary>
        /// Product images
        /// </summary>
        public List<CartProductImage> Images { get; set; } = new List<CartProductImage>();
    }

    /// <summary>
    /// Model for product category in cart
    /// </summary>
    public class CartProductCategory
    {
        /// <summary>
        /// Category ID
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Whether category is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Whether category has sub menu
        /// </summary>
        public bool SubMenu { get; set; }
    }

    /// <summary>
    /// Model for product image in cart
    /// </summary>
    public class CartProductImage
    {
        /// <summary>
        /// Image ID
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// Image poster/filename
        /// </summary>
        public string Poster { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is the main product image
        /// </summary>
        public bool Main { get; set; }

        /// <summary>
        /// Whether image is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Image display order
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// Full image URL
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for cart summary
    /// </summary>
    public class CartSummary
    {
        /// <summary>
        /// Total number of unique items in cart
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total quantity of all items
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Total amount of all items
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Total amount of available items only
        /// </summary>
        public decimal AvailableItemsTotal { get; set; }

        /// <summary>
        /// Number of unavailable items
        /// </summary>
        public int UnavailableItems { get; set; }
    }

    /// <summary>
    /// Model for recommended product
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
