using Newtonsoft.Json;
using System.Collections.Generic;
using Tenant.Query.Model.Response.ProductMaster;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tenant.Query.Model.Response
{
    public class CartList
    {
        [JsonProperty("productId")]
        public long ProductId { get; set; }                                                                                

        [JsonProperty("tenantId")]
        public long TenantId { get; set; }

        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("orderBy")]
        public long OrderBy { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("images")]
        public List<ProductImages> images { get; set; }
    }
}