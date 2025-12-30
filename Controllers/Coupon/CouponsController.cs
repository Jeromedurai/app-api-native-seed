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
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult
                    {
                        Exception = string.Join("; ", errors)
                    });
                }

                Logger.LogInformation($"Controller: Validating coupon - Code: {request.Code}, Amount: {request.Amount}, UserId: {request.UserId?.ToString() ?? "NULL"}, TenantId: {request.TenantId}");

                var response = await _couponService.ValidateCoupon(request);
                
                Logger.LogInformation($"Controller: Validation result - Valid: {response.Valid}, Message: {response.Message}");

                // Always return OK with the validation result, even if validation failed
                // The frontend will check the Valid property to determine success/failure
                return Ok(new ApiResult
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error validating coupon: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult
                    {
                        Exception = string.Join("; ", errors)
                    });
                }

                var coupon = await _couponService.CreateCoupon(request);

                return Ok(new ApiResult
                {
                    Data = coupon
                });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid argument: {ex.Message}");
                return BadRequest(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating coupon: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error getting coupons: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogWarning($"Coupon not found: {ex.Message}");
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting coupon: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Update coupon
        /// </summary>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="request">Update coupon request</param>
        /// <returns>Updated coupon</returns>
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

                request.CouponId = couponId;
                var coupon = await _couponService.UpdateCoupon(request);

                return Ok(new ApiResult
                {
                    Data = coupon
                });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid argument: {ex.Message}");
                return BadRequest(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning($"Coupon not found: {ex.Message}");
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating coupon: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete coupon (soft delete)
        /// </summary>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Success status</returns>
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
                Logger.LogError($"Error deleting coupon: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error getting coupon usage: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        #endregion
    }
}

