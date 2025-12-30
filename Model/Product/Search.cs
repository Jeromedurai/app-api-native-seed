using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    public class Search
    {
        [JsonProperty("category")]
        public List<string> Category { get; set; }

        [JsonProperty("subCategory")]
        public List<string> SubCategory { get; set; }

        [JsonProperty("isActive")]
        public bool isActive { get; set; }
    }
}
