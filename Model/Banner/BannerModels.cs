using System;
using Microsoft.AspNetCore.Http;

namespace Tenant.Query.Model.Banner
{
    /// <summary>Banner metadata returned to clients (no image bytes; image served by id).</summary>
    public class BannerResponse
    {
        public long BannerId { get; set; }
        public long TenantId { get; set; }
        public string Title { get; set; }
        public string CtaText { get; set; }
        public string CtaLink { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public long? FileSize { get; set; }
        public int OrderBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>Raw image bytes for the serve endpoint.</summary>
    public class BannerImageBinary
    {
        public long BannerId { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
    }

    /// <summary>Multipart create payload (image + campaign fields).</summary>
    public class CreateBannerRequest
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public string CtaText { get; set; }
        public string CtaLink { get; set; }
        public int OrderBy { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = true;
    }

    /// <summary>Multipart update payload. File is optional — omit to keep the existing image.</summary>
    public class UpdateBannerRequest
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public string CtaText { get; set; }
        public string CtaLink { get; set; }
        public int OrderBy { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = true;
    }
}
