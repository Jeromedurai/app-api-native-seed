using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Model.Validation;

namespace Tenant.Query.Model.Response
{
    public class Category
    {
        [JsonProperty("id")]
        public long CategoryId { get; set; }

        [JsonProperty("active")]
        public bool active { get; set; }

        [JsonProperty("subMenu")]
        public bool subMenu { get; set; }

        [JsonProperty("link")]
        public string link { get; set; }

        [JsonProperty("category")]
        public string Name { get; set; }        

        [JsonProperty("order")]
        public long Order { get; set; }

        [JsonProperty("tenantId")]
        public long tenantId { get; set; }

        [JsonProperty("subCategory")]
        public List<Model.Response.SubCategory> subCategories { get; set; }

        [JsonProperty("menuId")]
        public long? MenuId { get; set; }
    }

    public class CatrtegoryPayload
    {
        [JsonProperty("tenantId")]
        public long TenantId { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subCategory")]
        public bool SubCategory { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("subMenu")]
        public bool SubMenu { get; set; }

        [JsonProperty("orderBy")]
        public long OrderBy { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
