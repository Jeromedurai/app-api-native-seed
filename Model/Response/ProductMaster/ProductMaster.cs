using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tenant.Query.Model.Response.ProductMaster
{
    public class ProductMaster
    {
        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("tenantId")]
        public long TenantId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productDescription")]
        public string ProductDescription { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("fullDescription")]
        public string FullDescription { get; set; }

        [JsonProperty("specification")]
        public string Specification { get; set; }

        [JsonProperty("story")]
        public string Story { get; set; }

        [JsonProperty("packQuantity")]
        public long PackQuantity { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("rating")]
        public long Rating { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("trending")]
        public long Trending { get; set; }

        [JsonProperty("userBuyCount")]
        public long UserBuyCount { get; set; }

        [JsonProperty("return")]
        public long Return { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("modified")]
        public string Modified { get; set; }

        [JsonProperty("in_stock")]
        public bool In_stock { get; set; }

        [JsonProperty("best_seller")]
        public bool Best_seller { get; set; }        

        [JsonProperty("deleveryDate")]
        public long DeleveryDate { get; set; }

        [JsonProperty("offer")]
        public string Offer { get; set; }

        [JsonProperty("orderBy")]
        public long OrderBy { get; set; }

        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("long_description")]
        public string LongDescription { get; set; }

        [JsonProperty("images")]
        public List<ProductImages> images { get; set; }
    }

}
