using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tenant.Query.Model.WishList
{
    /// <summary>
    /// Request model for getting wishlist items
    /// </summary>
    public class GetWishlistItemsRequest
    {
        [JsonProperty("tenantId")]
        [Required(ErrorMessage = "Tenant ID is required")]
        public long TenantId { get; set; }

        [JsonProperty("userId")]
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        [JsonProperty("wishlist_id")]
        public long? WishlistId { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; } = 1;

        [JsonProperty("page_size")]
        public int PageSize { get; set; } = 100;

        [JsonProperty("sort_by")]
        public string SortBy { get; set; } = "added_at"; // added_at, priority, product_name, price

        [JsonProperty("sort_order")]
        public string SortOrder { get; set; } = "DESC"; // ASC, DESC
    }
}

