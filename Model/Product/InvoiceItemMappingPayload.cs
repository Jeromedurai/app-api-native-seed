using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    public class InvoiceItemMappingPayload
    {
        public InvoiceItemMappingPayload()
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
        public List<long> category { get; set; }
    }
}
