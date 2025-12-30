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
    [Table("ProductReview")]
    public class ProductReview : TnBase
    {
        [JsonProperty("rating"), Column("Rating")]
        public long Rating { get; set; }

        [JsonProperty("comment"), Column("Comment")]
        public string Comment { get; set; }

        [JsonProperty("productId"), Column("ProductId")]
        public long ProductId { get; set; }

        [JsonProperty("userId"), Column("UserId")]
        public long UserId { get; set; }

        [JsonProperty("active"), Column("Active")]
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
