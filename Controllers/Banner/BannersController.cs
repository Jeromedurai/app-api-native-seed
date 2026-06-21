using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.API.Base.Controller;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Banner;
using Tenant.Query.Service.Banner;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Banner
{
    /// <summary>
    /// Homepage / campaign banner management. Isolated from the product-image endpoints.
    /// Public: GET active list + GET image bytes. Admin: list/create/update/delete.
    /// </summary>
    [Route("api/1.0/banners")]
    public class BannersController : TnBaseController<BannerService>
    {
        private const long MaxFileSize = 10 * 1024 * 1024;
        private const string GenericErrorMessage = "An error occurred while processing your request.";
        private readonly BannerService _bannerService;

        public BannersController(
            BannerService service,
            IConfiguration configuration,
            ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {
            _bannerService = service;
        }

        #region Public

        /// <summary>Active banners for the homepage (date window + active), ordered by OrderBy.</summary>
        [AllowAnonymous]
        [HttpGet("active")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> GetActiveBanners([FromQuery] long tenantId)
        {
            try
            {
                var data = await _bannerService.GetActiveBanners(tenantId);
                return Ok(new ApiResult { Data = data });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting active banners for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = GenericErrorMessage });
            }
        }

        /// <summary>Serve banner image bytes by id (mirrors product image serve).</summary>
        [AllowAnonymous]
        [HttpGet("{bannerId:long}/image")]
        [ResponseCache(Duration = 86400)] // 1 day
        public async Task<IActionResult> GetBannerImage([FromRoute] long bannerId)
        {
            try
            {
                var image = await _bannerService.GetBannerImage(bannerId);
                if (image == null || image.ImageData == null) return NotFound();
                return File(image.ImageData, image.ContentType, image.ImageName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error serving banner image {BannerId}", bannerId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = GenericErrorMessage });
            }
        }

        #endregion

        #region Admin

        /// <summary>List all banners for a tenant (admin).</summary>
        [Authorize]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> GetBanners([FromQuery] long tenantId)
        {
            var tenantError = ValidateTenant(tenantId);
            if (tenantError != null) return tenantError;

            try
            {
                var data = await _bannerService.GetBannersAdmin(tenantId);
                return Ok(new ApiResult { Data = data });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting banners for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = GenericErrorMessage });
            }
        }

        /// <summary>Create a banner (multipart: image + campaign fields).</summary>
        [Authorize]
        [HttpPost("tenants/{tenantId:long}/upload")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> CreateBanner([FromRoute] long tenantId, [FromForm] CreateBannerRequest request)
        {
            var tenantError = ValidateTenant(tenantId);
            if (tenantError != null) return tenantError;

            try
            {
                if (request == null) return BadRequest(new ApiResult { Exception = "Request data is missing" });
                var data = await _bannerService.CreateBanner(tenantId, request, GetUserId());
                return Ok(new ApiResult { Data = data });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating banner for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = GenericErrorMessage });
            }
        }

        /// <summary>Update a banner. Re-uploading the image is optional. POST (multipart) so the
        /// existing postAuthFormData client helper can be reused.</summary>
        [Authorize]
        [HttpPost("{bannerId:long}/tenants/{tenantId:long}/update")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> UpdateBanner([FromRoute] long bannerId, [FromRoute] long tenantId, [FromForm] UpdateBannerRequest request)
        {
            var tenantError = ValidateTenant(tenantId);
            if (tenantError != null) return tenantError;

            try
            {
                if (request == null) return BadRequest(new ApiResult { Exception = "Request data is missing" });
                var data = await _bannerService.UpdateBanner(bannerId, tenantId, request, GetUserId());
                return Ok(new ApiResult { Data = data });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating banner {BannerId} for tenant {TenantId}", bannerId, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = GenericErrorMessage });
            }
        }

        /// <summary>Delete a banner.</summary>
        [Authorize]
        [HttpDelete("{bannerId:long}/tenants/{tenantId:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> DeleteBanner([FromRoute] long bannerId, [FromRoute] long tenantId)
        {
            var tenantError = ValidateTenant(tenantId);
            if (tenantError != null) return tenantError;

            try
            {
                var ok = await _bannerService.DeleteBanner(bannerId, tenantId);
                if (!ok) return NotFound(new ApiResult { Exception = $"Banner {bannerId} not found." });
                return Ok(new ApiResult { Data = new { success = true } });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting banner {BannerId} for tenant {TenantId}", bannerId, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = GenericErrorMessage });
            }
        }

        #endregion

        /// <summary>Best-effort current user id from JWT claims (nullable; SP accepts null).</summary>
        private long? GetUserId()
        {
            var claim = User?.FindFirst("user_id")?.Value
                ?? User?.FindFirst("userId")?.Value
                ?? User?.FindFirst("sub")?.Value;
            return long.TryParse(claim, out var id) ? id : (long?)null;
        }

        /// <summary>
        /// Validate that the route/query tenantId is positive AND matches the caller's tenant claim.
        /// Returns an error <see cref="IActionResult"/> to short-circuit, or null when authorized.
        /// Uses the same claim reads as ProductsController (the proven, working tenant check).
        /// </summary>
        private IActionResult ValidateTenant(long tenantId)
        {
            if (tenantId <= 0)
            {
                return BadRequest(new ApiResult { Exception = "A valid tenantId is required." });
            }

            var tokenTenant = User?.FindFirst("tenant_id")?.Value
                ?? User?.FindFirst("tenantId")?.Value;
            if (string.IsNullOrEmpty(tokenTenant) || tokenTenant != tenantId.ToString())
            {
                return Forbid(); // 403 — caller does not own this tenant
            }

            return null;
        }
    }
}
