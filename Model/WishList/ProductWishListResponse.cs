using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Tenant.Query.Model.Product;

namespace Tenant.Query.Model.WishList
{
    public class ProductWishListResponse
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }

        [JsonProperty("name")]
        public string ProductName { get; set; }

        [JsonProperty("images")]
        public List<ImageResponseDto> images { get; set; }
    }
}
