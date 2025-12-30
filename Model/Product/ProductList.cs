using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Tenant.Query.Model.Product
{
    public class ProductImages
    {
        [JsonProperty("imageName"), Column("ImageName")]
        public string ImageName { get; set; }

        [JsonProperty("contentType"), Column("ContentType")]
        public string ContentType { get; set; }

        [JsonProperty("imageData "), Column("ImageData ")]
        public byte[] ImageData { get; set; }

        [JsonProperty("thumbnailData"), Column("ThumbnailData")]
        public byte[] ThumbnailData { get; set; }

        [JsonProperty("ProductId"), Column("ProductId")]
        public long ProductId { get; set; }

        [JsonProperty("FileSize"), Column("FileSize")]
        public int FileSize { get; set; }

        [JsonProperty("Active"), Column("Active")]
        public bool Active { get; set; }

        [JsonProperty("guid"), Column("Guid")]
        public Guid Guid { get; set; }

        [JsonProperty("created"), Column("Created")]
        public DateTime Created { get; set; }
    }
}
