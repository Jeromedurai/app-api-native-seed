using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Controller;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Coupon;
using Tenant.Query.Service.Coupon;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Coupon
{
    [Route("api/1.0/coupons")]
    public class CouponsController : TnBaseController<CouponService>
    {
        #region Initialize
        private const string GenericErrorMessage = "An error occurred while processing your request.";
        private readonly CouponService _couponService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public CouponsController(
            CouponService service,
            IConfiguration configuration,
            ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {
            _couponService = service;
        }

        /// <summary>
        /// Builds a 400 response from the current ModelState validation errors,
        /// preserving the existing "; "-joined message format.
        /// </summary>
        private IActionResult ModelStateError()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new ApiResult
            {
                Exception = string.Join("; ", errors)
            });
        }

        /// <summary>
        /// Best-effort tenant id from JWT claims. Reads the claim-name variations used across
        /// this codebase (canonical names live in a compiled DLL). Null when no readable claim.
        /// </summary>
        private long? GetTenantIdFromJwt()
        {
            var claim = User?.FindFirst("tenant_id")?.Value
                ?? User?.FindFirst("tenantId")?.Value;
            return long.TryParse(claim, out var id) ? id : (long?)null;
        }

        /// <summary>
        /// Enforces that a client-supplied tenantId matches the authenticated caller's JWT tenant.
        /// Returns a 403 result to short-circuit on mismatch, or null when access is allowed.
        /// Defensive: if the token carries no readable tenant claim, the request proceeds (falls
        /// back to the supplied id) so authenticated calls never hard-fail on claim-name
        /// uncertainty. Callers must already require [Authorize].
        /// </summary>
        private IActionResult ValidateTenantAccess(long tenantId)
        {
            var tokenTenantId = GetTenantIdFromJwt();
            if (tokenTenantId.HasValue && tokenTenantId.Value != tenantId)
            {
                Logger.LogWarning(
                    "Tenant access denied: token tenant {TokenTenantId} attempted to act on tenant {RequestTenantId}",
                    tokenTenantId.Value, tenantId);
                return Forbid(); // 403 — caller does not own this tenant
            }

            return null;
        }

        #region Public Endpoints

        /// <summary>
        /// Validate/Check coupon code
        /// </summary>
        /// <param name="request">Validate coupon request</param>
        /// <returns>Coupon validation response</returns>
        [HttpPost]
        [Route("validate")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Request body is required"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return ModelStateError();
                }

                Logger.LogInformation("Validating coupon {Code} for tenant {TenantId}", request.Code, request.TenantId);

                var response = await _couponService.ValidateCoupon(request);

                // Always return OK with the validation result, even if validation failed
                // The frontend will check the Valid property to determine success/failure
                return Ok(new ApiResult
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating coupon");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        #endregion

        #region Admin Endpoints

        /// <summary>
        /// Create a new coupon
        /// </summary>
        /// <param name="request">Create coupon request</param>
        /// <returns>Created coupon</returns>
        [Authorize]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Request body is required"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return ModelStateError();
                }

                var accessError = ValidateTenantAccess(request.TenantId);
                if (accessError != null) return accessError;

                var coupon = await _couponService.CreateCoupon(request);

                return Ok(new ApiResult
                {
                    Data = coupon
                });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning(ex, "Invalid argument creating coupon");
                return BadRequest(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating coupon");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Get all coupons for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of all coupons</returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetCoupons([FromQuery] long tenantId)
        {
            try
            {
                var coupons = await _couponService.GetCoupons(tenantId);

                return Ok(new ApiResult
                {
                    Data = coupons
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting coupons for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Get coupon by ID
        /// </summary>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <returns>Coupon details</returns>
        [HttpGet]
        [Route("{couponId:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetCouponById(
            [FromRoute] long couponId,
            [FromQuery] long? tenantId = null)
        {
            try
            {
                var coupon = await _couponService.GetCouponById(couponId, tenantId);

                return Ok(new ApiResult
                {
                    Data = coupon
                });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning(ex, "Coupon {CouponId} not found", couponId);
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting coupon {CouponId}", couponId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Update coupon
        /// </summary>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="request">Update coupon request</param>
        /// <returns>Updated coupon</returns>
        [Authorize]
        [HttpPost]
        [Route("{couponId:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UpdateCoupon(
            [FromRoute] long couponId,
            [FromBody] UpdateCouponRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Invalid request parameters"
                    });
                }

                var accessError = ValidateTenantAccess(request.TenantId);
                if (accessError != null) return accessError;

                request.CouponId = couponId;
                var coupon = await _couponService.UpdateCoupon(request);

                return Ok(new ApiResult
                {
                    Data = coupon
                });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning(ex, "Invalid argument updating coupon {CouponId}", couponId);
                return BadRequest(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning(ex, "Coupon {CouponId} not found", couponId);
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating coupon {CouponId}", couponId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Delete coupon (soft delete)
        /// </summary>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Success status</returns>
        [Authorize]
        [HttpDelete]
        [Route("{couponId:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> DeleteCoupon(
            [FromRoute] long couponId,
            [FromQuery] long tenantId)
        {
            try
            {
                var accessError = ValidateTenantAccess(tenantId);
                if (accessError != null) return accessError;

                var result = await _couponService.DeleteCoupon(couponId, tenantId);

                if (!result)
                {
                    return NotFound(new ApiResult
                    {
                        Exception = "Coupon not found"
                    });
                }

                return Ok(new ApiResult
                {
                    Data = new { Message = "Coupon deleted successfully" }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting coupon {CouponId}", couponId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Get coupon usage statistics
        /// </summary>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <returns>Coupon usage details and statistics</returns>
        [HttpGet]
        [Route("{couponId:long}/usage")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetCouponUsage(
            [FromRoute] long couponId,
            [FromQuery] long? tenantId = null)
        {
            try
            {
                var (usages, statistics) = await _couponService.GetCouponUsage(couponId, tenantId);

                return Ok(new ApiResult
                {
                    Data = new
                    {
                        Usages = usages,
                        Statistics = statistics
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting coupon usage {CouponId}", couponId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        #endregion
    }
}

