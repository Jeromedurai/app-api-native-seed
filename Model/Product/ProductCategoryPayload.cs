using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;

namespace Tenant.Query.Model.Product
{
    public class ProductCategoryPayload
    {
        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("categoryId")]
        public List<long> CategoryId { get; set; }
    }
}
