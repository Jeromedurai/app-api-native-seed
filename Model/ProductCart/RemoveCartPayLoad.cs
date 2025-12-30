using Newtonsoft.Json;

namespace Tenant.Query.Model.ProductCart
{
    public class RemoveCartPayLoad
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("productId")]
        public long ProductId { get; set; }
    }
}
