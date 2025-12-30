using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Tenant.Query.Model.Product
{
    //public sealed class ProductMasterList
    //{
    //    [JsonProperty("productItems")]
    //    public List<ProductItemList> productItemList { get; set; }

    //    [JsonProperty("totalRowCount"), Column("TotalRowCount")]
    //    public int? TotalRowCount { get; set; }
    //}
    public class ProductItemList
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("name"), Column("ProductName")]
        public string ProductName { get; set; }

        [JsonProperty("displayname"), Column("Displayname")]
        public string Displayname { get; set; }

        [JsonProperty("description"), Column("Description")]
        public string Description { get; set; }

        [JsonProperty("price"), Column("Price")]
        public decimal Price { get; set; }

        [JsonProperty("stock"), Column("Stock")]
        public long Stock { get; set; }

        [JsonProperty("bestSeller"), Column("BestSeller")]
        public bool BestSeller { get; set; }

        [JsonProperty("quantity"), Column("Quantity")]
        public long Quantity { get; set; }

        [JsonProperty("numofReviews"), Column("Numofreviews")]
        public long? Numofreviews { get; set; }

        [JsonProperty("active"), Column("Active")]
        public bool Active { get; set; }

        [JsonProperty("tenantId"), Column("TenantId")]
        public string TenantId { get; set; }

        [JsonProperty("created"), Column("Created")]
        public DateTime Created { get; set; }

        [JsonProperty("createdBy"), NotMapped]
        public string CreatedBy { get; set; }

        [JsonProperty("lastModified"), Column("LastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("lastModifiedBy"), NotMapped]
        public string LastModifiedBy { get; set; }

        [JsonProperty("ProductId"), Column("ProductId")]
        public long ProductId { get; set; }

        [JsonProperty("rating"), Column("Rating")]
        public long Rating { get; set; }

        [JsonProperty("comment"), Column("Comment")]
        public string Comment { get; set; }

        [JsonProperty("userId"), Column("UserId")]
        public long UserId { get; set; }

        [JsonProperty("images"), Column("images")]
        public List<Model.Product.ImageResponseDto> images { get; set; }        
    }

    //public class ProductImages
    //{
    //    [JsonProperty("url"), Column("Url")]
    //    public string Url { get; set; }

    //    [JsonProperty("poster"), Column("Poster")]
    //    public string Poster { get; set; }

    //    [JsonProperty("Title"), Column("Title")]
    //    public string Title { get; set; }

    //    [JsonProperty("ProductId"), Column("ProductId")]
    //    public long ProductId { get; set; }

    //    [JsonProperty("orderBy"), Column("OrderBy")]
    //    public long OrderBy { get; set; }
    //}
    public class SpProductMasterList
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

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

        [JsonProperty("bestSeller"), Column("BestSeller")]
        public bool BestSeller { get; set; }        

        [JsonProperty("quantity"), Column("Quantity")]
        public long Quantity { get; set; }

        [JsonProperty("numofReviews"), Column("Numofreviews")]
        public long? Numofreviews { get; set; }        

        [JsonProperty("active"), Column("Active")]
        public bool Active { get; set; }

        [JsonProperty("tenantId"), Column("TenantId")]
        public string TenantId { get; set; }

        [JsonProperty("created"), Column("Created")]
        public DateTime Created { get; set; }

        [JsonProperty("createdBy"), NotMapped]
        public string CreatedBy { get; set; }

        [JsonProperty("lastModified"), Column("LastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("lastModifiedBy"), NotMapped]
        public string LastModifiedBy { get; set; }

        [JsonProperty("url"), Column("Url")]
        public string Url { get; set; }

        [JsonProperty("poster"), Column("Poster")]
        public string Poster { get; set; }

        [JsonProperty("Id"), Column("Id")]
        public long Id { get; set; }

        [JsonProperty("Title"), Column("Title")]
        public string Title { get; set; }

        [JsonProperty("ProductId"), Column("ProductId")]
        public long ProductId { get; set; }

        [JsonProperty("rating"), Column("Rating")]
        public long Rating { get; set; }

        [JsonProperty("comment"), Column("Comment")]
        public string Comment { get; set; }

        [JsonProperty("userId"), Column("UserId")]
        public long UserId { get; set; }

        [JsonProperty("orderBy"), Column("OrderBy")]
        public long OrderBy { get; set; }

        [JsonProperty("contentType"), Column("ContentType")]
        public string ContentType { get; set; }

        [JsonProperty("imageName"), Column("ImageName")]
        public string ImageName { get; set; }

        [JsonProperty("totalRowCount"), Column("TotalRowCount")]
        public int? TotalRowCount { get; set; }
    }
}
