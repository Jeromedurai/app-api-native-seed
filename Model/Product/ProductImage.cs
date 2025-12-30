//using iTextSharp.text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tenant.API.Base.Core;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Tenant.Query.Model.Product
{
    [Table("ProductImage")]
    public class ProductImage : TnBase
    {
        [JsonProperty("url"), Column("Url")]
        public string Url { get; set; }

        [JsonProperty("Title"), Column("Title")]
        public string Title { get; set; }

        [JsonProperty("ProductId"), Column("ProductId")]
        public long ProductId { get; set; }

        [JsonProperty("Active"), Column("Active")]
        public bool Active { get; set; }

        [JsonIgnore, NotMapped]
        public override string Guid { get; set; }

        [JsonIgnore, NotMapped]
        public override DateTime Created { get; set; }

        [JsonIgnore, NotMapped]
        public override string CreatedBy { get; set; }

        [JsonIgnore, NotMapped]
        public override DateTime LastModified { get; set; }

        [JsonIgnore, NotMapped]
        public override string LastModifiedBy { get; set; }

        [JsonIgnore, NotMapped]
        public override string TenantId { get; set; }
    }
}
