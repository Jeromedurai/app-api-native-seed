using Newtonsoft.Json;

namespace Tenant.Query.Model.ProductCart
{
    public class CartPayload
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }
    }
}
