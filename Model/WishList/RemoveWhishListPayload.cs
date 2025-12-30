using Newtonsoft.Json;

namespace Tenant.Query.Model.WishList
{
    public class RemoveWhishListPayload
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("productId")]
        public long ProductId { get; set; }
    }
}
