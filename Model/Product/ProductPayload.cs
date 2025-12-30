using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Tenant.Query.Model.Product
{
    public class ProductPayload
    {
        public ProductPayload()
        {
            Page = 1;
            PageSize = 10;
        }

        [JsonProperty("roleId")]
        public string RoleId { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("orderBy")]
        public string OrderBy { get; set; }

        [JsonProperty("search")]
        public string Search { get; set; }

        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("productId")]
        public List<long> ProductId { get; set; }

        public string ConvertVendorListToString()
        {
            if (ProductId == null || !ProductId.Any())
            {
                return "0";
            }
            return string.Join(",", ProductId);
        }
    }
}
