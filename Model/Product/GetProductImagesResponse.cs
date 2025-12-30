using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Model for product image in the response
    /// </summary>
    public class ProductImageResponse
    {
        /// <summary>
        /// Image ID
        /// </summary>
        [JsonProperty("imageId")]
        public long ImageId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        [JsonProperty("productId")]
        public long ProductId { get; set; }

        /// <summary>
        /// Image URL
        /// </summary>
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Thumbnail URL
        /// </summary>
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Whether this is the main image
        /// </summary>
        [JsonProperty("main")]
        public bool Main { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        [JsonProperty("orderBy")]
        public int OrderBy { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        [JsonProperty("modified")]
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Whether the image is active
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        [JsonProperty("tenantId")]
        public long TenantId { get; set; }
    }
}
