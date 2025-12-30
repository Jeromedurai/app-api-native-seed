using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.Product
{
    public class ProductCategory
    {
        [JsonProperty("CategoryId"), Column("CategoryId"), Key]
        public long CategoryId { get; set; }

        [JsonProperty("Category"), Column("Category")]
        public string Category { get; set; }

        [JsonProperty("link"), Column("link")]
        public string Link { get; set; }

        [JsonProperty("subCategory"), Column("SubCategory")]
        public long SubCategory { get; set; }

        [JsonProperty("orderBy"), Column("OrderBy")]
        public int OrderBy { get; set; }

        [JsonProperty("subMenu"), Column("SubMenu")]
        public bool SubMenu { get; set; }

        [JsonProperty("active"), Column("Active")]
        public bool Active { get; set; }

        [JsonProperty("tenantId"), Column("TenantId")]
        public string TenantId { get; set; }

        [JsonProperty("menuId"), Column("MenuId")]
        public long? MenuId { get; set; }
    }
}
