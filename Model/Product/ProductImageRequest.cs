using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Model for add product images request (multipart/form-data)
    /// </summary>
    public class AddProductImagesRequest
    {
        /// <summary>
        /// Product ID (will be set from route parameter)
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// User ID (optional, for activity logging)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Image files to upload
        /// </summary>
        [Required(ErrorMessage = "At least one image file is required")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        /// <summary>
        /// Whether any of these images should be set as main
        /// </summary>
        public bool Main { get; set; } = false;

        /// <summary>
        /// Display order for the images
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "OrderBy must be a non-negative number")]
        public int OrderBy { get; set; } = 0;

        /// <summary>
        /// IP address (will be extracted from request)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent (will be extracted from request)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for add product images response
    /// </summary>
    public class AddProductImagesResponse
    {
        /// <summary>
        /// List of successfully added images
        /// </summary>
        public List<ProductImageInfo> Images { get; set; } = new List<ProductImageInfo>();

        /// <summary>
        /// Total number of images added
        /// </summary>
        public int TotalAdded { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Model for update product image request
    /// </summary>
    public class UpdateProductImageRequest
    {
        /// <summary>
        /// Product ID (will be set from route parameter)
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Image ID (will be set from route parameter)
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// User ID (optional, for activity logging)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Whether this should be the main image
        /// </summary>
        public bool? Main { get; set; }

        /// <summary>
        /// Whether the image is active
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "OrderBy must be a non-negative number")]
        public int? OrderBy { get; set; }

        /// <summary>
        /// IP address (will be extracted from request)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent (will be extracted from request)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for update product image response
    /// </summary>
    public class UpdateProductImageResponse
    {
        /// <summary>
        /// Updated image information
        /// </summary>
        public ProductImageInfo Image { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Model for set main product image request
    /// </summary>
    public class SetMainProductImageRequest
    {
        /// <summary>
        /// Product ID (will be set from route parameter)
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Image ID (will be set from route parameter)
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// User ID (optional, for activity logging)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// IP address (will be extracted from request)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent (will be extracted from request)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for set main product image response
    /// </summary>
    public class SetMainProductImageResponse
    {
        /// <summary>
        /// Updated image information
        /// </summary>
        public ProductImageInfo Image { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool Success { get; set; }
    }

    /// <summary>
    /// Model for delete product image request
    /// </summary>
    public class DeleteProductImageRequest
    {
        /// <summary>
        /// Product ID (will be set from route parameter)
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Image ID (will be set from route parameter)
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// User ID (optional, for activity logging)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Whether to perform hard delete (permanent) or soft delete
        /// </summary>
        public bool HardDelete { get; set; } = false;

        /// <summary>
        /// IP address (will be extracted from request)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent (will be extracted from request)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for delete product image response
    /// </summary>
    public class DeleteProductImageResponse
    {
        /// <summary>
        /// Deleted image ID
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Deleted image name
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Whether the deleted image was the main image
        /// </summary>
        public bool WasMain { get; set; }

        /// <summary>
        /// Whether it was a hard delete
        /// </summary>
        public bool HardDeleted { get; set; }

        /// <summary>
        /// Deletion timestamp
        /// </summary>
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// User who performed the deletion
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Number of remaining active images
        /// </summary>
        public int RemainingActiveImages { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Remaining active images for the product
        /// </summary>
        public List<ProductImageInfo> RemainingImages { get; set; } = new List<ProductImageInfo>();

        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Model for product image information
    /// </summary>
    public class ProductImageInfo
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
        /// Image URL/poster
        /// </summary>
        [JsonProperty("poster")]
        public string Poster { get; set; }

        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Whether this is the main image
        /// </summary>
        [JsonProperty("main")]
        public bool Main { get; set; }

        /// <summary>
        /// Whether the image is active
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

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
        public DateTime Modified { get; set; }

        /// <summary>
        /// Image name/filename
        /// </summary>
        [JsonProperty("imageName")]
        public string ImageName { get; set; }

        /// <summary>
        /// Content type (MIME type)
        /// </summary>
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonProperty("fileSize")]
        public long FileSize { get; set; }
    }

    /// <summary>
    /// Model for image upload data (used internally for JSON serialization)
    /// </summary>
    public class ImageUploadData
    {
        /// <summary>
        /// Image name/filename
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Content type (MIME type)
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Base64 encoded image data
        /// </summary>
        public string ImageData { get; set; }

        /// <summary>
        /// Base64 encoded thumbnail data (optional)
        /// </summary>
        public string ThumbnailData { get; set; }

        /// <summary>
        /// Whether this is the main image
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public int OrderBy { get; set; }
    }
}
