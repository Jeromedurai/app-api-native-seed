using Microsoft.AspNetCore.Http;
using System;

namespace Tenant.Query.Model.Product
{
    public class ProductNewImage
    {
        public Guid Id { get; set; }
        public long ProductId { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] ThumbnailData { get; set; }
        public DateTime CreatedAt { get; set; }
        public int FileSize { get; set; }
        public bool IsActive { get; set; }
    }

    public class ImageUploadDto
    {
        public long ProductId { get; set; }
        public IFormFile File { get; set; }
        public bool GenerateThumbnail { get; set; } = true;
        public int? ThumbnailWidth { get; set; } = 200;
        public int? ThumbnailHeight { get; set; } = 200;
    }

    public class ImageResponseDto
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public int FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
