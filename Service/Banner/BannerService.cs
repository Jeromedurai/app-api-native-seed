using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tenant.API.Base.Repository;
using Tenant.API.Base.Service;
using Tenant.Query.Model.Banner;
using Tenant.Query.Repository.Banner;

namespace Tenant.Query.Service.Banner
{
    /// <summary>
    /// Business logic for homepage/campaign banners. Reads/writes image bytes via the
    /// banner repository (separate from the product-image pipeline).
    /// </summary>
    public class BannerService : TnBaseService
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
        private static readonly string[] AllowedContentTypes =
            { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

        private readonly BannerRepository _bannerRepository;

        public BannerService(
            BannerRepository bannerRepository,
            ILoggerFactory loggerFactory,
            TnAudit xcAudit,
            TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            _bannerRepository = bannerRepository;
            _bannerRepository.Logger = loggerFactory.CreateLogger<BannerRepository>();
            this.Logger = loggerFactory.CreateLogger<BannerService>();
        }

        public Task<List<BannerResponse>> GetBannersAdmin(long tenantId) => _bannerRepository.GetBannersAdmin(tenantId);

        public Task<List<BannerResponse>> GetActiveBanners(long tenantId) => _bannerRepository.GetActiveBanners(tenantId);

        public Task<BannerImageBinary> GetBannerImage(long bannerId) => _bannerRepository.GetBannerImage(bannerId);

        public async Task<BannerResponse> CreateBanner(long tenantId, CreateBannerRequest request, long? createdBy)
        {
            var (bytes, contentType, name) = await ReadAndValidateFile(request.File, required: true);
            return await _bannerRepository.CreateBanner(
                tenantId, request, name, contentType, bytes.LongLength, bytes, createdBy);
        }

        public async Task<BannerResponse> UpdateBanner(long bannerId, long tenantId, UpdateBannerRequest request, long? updatedBy)
        {
            // File optional on update — when absent, keep the existing image (repo/SP handle null).
            if (request.File != null && request.File.Length > 0)
            {
                var (bytes, contentType, name) = await ReadAndValidateFile(request.File, required: true);
                return await _bannerRepository.UpdateBanner(
                    bannerId, tenantId, request, name, contentType, bytes.LongLength, bytes, updatedBy);
            }
            return await _bannerRepository.UpdateBanner(
                bannerId, tenantId, request, null, null, null, null, updatedBy);
        }

        public Task<bool> DeleteBanner(long bannerId, long tenantId) => _bannerRepository.DeleteBanner(bannerId, tenantId);

        /// <summary>Reads the uploaded file into bytes and validates type/size/signature.</summary>
        private static async Task<(byte[] bytes, string contentType, string name)> ReadAndValidateFile(IFormFile file, bool required)
        {
            if (file == null || file.Length == 0)
            {
                if (required) throw new ArgumentException("No image file uploaded.");
                return (null, null, null);
            }
            if (file.Length > MaxFileSize)
                throw new ArgumentException($"File size exceeds {MaxFileSize / (1024 * 1024)}MB limit.");

            var contentType = file.ContentType?.ToLowerInvariant();
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var typeOk = (!string.IsNullOrEmpty(contentType) && AllowedContentTypes.Contains(contentType))
                || (!string.IsNullOrEmpty(extension) && validExtensions.Contains(extension));
            if (!typeOk)
                throw new ArgumentException($"Invalid file type. Allowed: JPEG, PNG, GIF, WebP. Received: {contentType ?? "unknown"}");

            byte[] bytes;
            using (var stream = file.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            if (bytes.Length < 100)
                throw new ArgumentException("File is too small to be a valid image.");
            if (!HasValidImageSignature(bytes))
                throw new ArgumentException("Uploaded file is not a recognised image format.");

            var name = string.IsNullOrWhiteSpace(file.FileName) ? "banner" : file.FileName;
            return (bytes, string.IsNullOrEmpty(contentType) ? "image/jpeg" : contentType, name);
        }

        /// <summary>Magic-number check for JPEG/PNG/GIF/WebP (self-contained; no ProductService dependency).</summary>
        private static bool HasValidImageSignature(byte[] d)
        {
            if (d.Length >= 3 && d[0] == 0xFF && d[1] == 0xD8 && d[2] == 0xFF) return true;                 // JPEG
            if (d.Length >= 4 && d[0] == 0x89 && d[1] == 0x50 && d[2] == 0x4E && d[3] == 0x47) return true;  // PNG
            if (d.Length >= 6 && d[0] == 0x47 && d[1] == 0x49 && d[2] == 0x46 && d[3] == 0x38
                && (d[4] == 0x37 || d[4] == 0x39) && d[5] == 0x61) return true;                              // GIF
            if (d.Length >= 12 && d[0] == 0x52 && d[1] == 0x49 && d[2] == 0x46 && d[3] == 0x46
                && d[8] == 0x57 && d[9] == 0x45 && d[10] == 0x42 && d[11] == 0x50) return true;              // WebP
            return false;
        }
    }
}
