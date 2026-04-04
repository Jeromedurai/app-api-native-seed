using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    public class AddConvertedProductImageRequest
    {
        public long ProductId { get; set; }

        [Required(ErrorMessage = "Image file is required")]
        public IFormFile Image { get; set; }

        public long? UserId { get; set; }

        public bool Main { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "OrderBy must be a non-negative number")]
        public int OrderBy { get; set; }

        public string QualityProfile { get; set; }
    }

    public class AddConvertedProductImageResponse
    {
        public long ConvertedImageId { get; set; }
        public long ProductId { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public bool Main { get; set; }
        public bool Active { get; set; }
        public int OrderBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OriginalUrl { get; set; }
        public string LargeUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public int LargeWidth { get; set; }
        public int LargeHeight { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public long OriginalFileSize { get; set; }
        public long LargeFileSize { get; set; }
        public long ThumbnailFileSize { get; set; }
        public string QualityProfileApplied { get; set; }
        public int JpegQualityApplied { get; set; }
        public int WebpQualityApplied { get; set; }
        public bool ThumbnailSharpenApplied { get; set; }
        public double ThumbnailSharpenSigmaApplied { get; set; }
    }

    public class ProductConvertedImageBinary
    {
        public long ConvertedImageId { get; set; }
        public long ProductId { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public byte[] OriginalData { get; set; }
        public byte[] LargeData { get; set; }
        public byte[] ThumbnailData { get; set; }
        public bool Main { get; set; }
        public bool Active { get; set; }
        public int OrderBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
