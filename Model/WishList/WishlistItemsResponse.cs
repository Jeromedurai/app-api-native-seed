using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tenant.Query.Model.WishList
{
    /// <summary>
    /// Response model for wishlist items with product details
    /// </summary>
    public class WishlistItemsResponse
    {
        [JsonProperty("items")]
        public List<WishlistItemWithProduct> Items { get; set; } = new List<WishlistItemWithProduct>();

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; } = 1;

        [JsonProperty("page_size")]
        public int PageSize { get; set; } = 100;

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("has_next")]
        public bool HasNext { get; set; }

        [JsonProperty("has_previous")]
        public bool HasPrevious { get; set; }
    }

    /// <summary>
    /// Wishlist item with full product details
    /// </summary>
    public class WishlistItemWithProduct
    {
        [JsonProperty("wishlist_item_id")]
        public long WishlistItemId { get; set; }

        [JsonProperty("wishlist_id")]
        public long WishlistId { get; set; }

        [JsonProperty("product_id")]
        public long ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; } = 1;

        [JsonProperty("priority")]
        public int Priority { get; set; } = 3; // 1=High, 2=Medium, 3=Low

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("notify_on_sale")]
        public bool NotifyOnSale { get; set; } = false;

        [JsonProperty("notify_on_restock")]
        public bool NotifyOnRestock { get; set; } = false;

        [JsonProperty("notify_on_price_drop")]
        public bool NotifyOnPriceDrop { get; set; } = false;

        [JsonProperty("price_alert_threshold")]
        public decimal? PriceAlertThreshold { get; set; }

        [JsonProperty("added_at")]
        public DateTime AddedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("product")]
        public WishlistProduct Product { get; set; }

        [JsonProperty("alerts")]
        public WishlistAlerts Alerts { get; set; } = new WishlistAlerts();
    }

    /// <summary>
    /// Product information for wishlist item
    /// </summary>
    public class WishlistProduct
    {
        [JsonProperty("product_id")]
        public long ProductId { get; set; }

        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("product_description")]
        public string ProductDescription { get; set; }

        [JsonProperty("current_price")]
        public decimal CurrentPrice { get; set; }

        [JsonProperty("original_price")]
        public decimal? OriginalPrice { get; set; }

        [JsonProperty("in_stock")]
        public bool InStock { get; set; }

        [JsonProperty("stock_quantity")]
        public int StockQuantity { get; set; }

        [JsonProperty("rating")]
        public decimal Rating { get; set; }

        [JsonProperty("total_reviews")]
        public int TotalReviews { get; set; }

        [JsonProperty("discount_percentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonProperty("offer_text")]
        public string OfferText { get; set; }

        [JsonProperty("product_image")]
        public string ProductImage { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Alerts for wishlist item
    /// </summary>
    public class WishlistAlerts
    {
        [JsonProperty("price_alert_triggered")]
        public bool PriceAlertTriggered { get; set; }

        [JsonProperty("back_in_stock")]
        public bool BackInStock { get; set; }

        [JsonProperty("on_sale")]
        public bool OnSale { get; set; }

        [JsonProperty("price_dropped")]
        public bool PriceDropped { get; set; }
    }
}

