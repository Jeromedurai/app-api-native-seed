using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.Product
{
    [Table("ProductMaster")]
    public class ProductMaster : TnBase
    {
        [JsonProperty("guid"), NotMapped]
        public override string Guid { get; set; }

        [JsonProperty("name"), Column("ProductName")]
        public string ProductName { get; set; }

        [JsonProperty("displayname"), Column("Displayname")]
        public string Displayname { get; set; }

        [JsonProperty("description"), Column("Description")]
        public string Description { get; set; }

        [JsonProperty("rating"), Column("Rating")]
        public long? rating { get; set; }

        [JsonProperty("price"), Column("Price")]
        public decimal Price { get; set; }

        [JsonProperty("stock"), Column("Stock")]
        public long Stock { get; set; }        

        [JsonProperty("quantity"), Column("Quantity")]
        public long Quantity { get; set; }

        [JsonProperty("numofReviews"), Column("Numofreviews")]
        public long? Numofreviews { get; set; }

        [JsonProperty("category"), Column("Category")]
        public long? Category { get; set; }

        [JsonProperty("subCategory"), Column("SubCategory")]
        public long? SubCategory { get; set; }

        [JsonProperty("active"), Column("Active")]
        public bool Active { get; set; }

        [JsonProperty("tenantId"), Column("TenantId")]
        public override string TenantId { get; set; }

        [JsonProperty("created"), Column("Created")]
        public override DateTime Created { get; set; }

        [JsonProperty("createdBy"), NotMapped]
        public override string CreatedBy { get; set; }

        [JsonProperty("lastModified"), Column("LastModified")]
        public override DateTime LastModified { get; set; }

        [JsonProperty("lastModifiedBy"), NotMapped]
        public override string LastModifiedBy { get; set; }

        [JsonProperty("images")]
        public List<Model.Product.ProductImage> ProductImages { get; set; }

        [JsonProperty("reviews")]
        public List<Model.Product.ProductReview> ProductReviews { get; set; }
    }
}
