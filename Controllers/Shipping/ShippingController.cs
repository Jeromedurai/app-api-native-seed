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
using Tenant.Query.Model.Shipping;
using Tenant.Query.Service.Shipping;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Shipping
{
    [Route("api/1.0/shipping")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        #region Initialize
        private readonly ShippingService _shippingService;
        public ILogger Logger { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ShippingController(
            ShippingService service,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _shippingService = service;
            Logger = loggerFactory.CreateLogger<ShippingController>();
        }

        #region Public Endpoints

        /// <summary>
        /// Get all states
        /// </summary>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <param name="countryCode">Country code (default: IN)</param>
        /// <param name="activeOnly">Return only active states (default: true)</param>
        /// <returns>List of states</returns>
        [HttpGet]
        [Route("states")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetStates(
            [FromQuery] int? tenantId = null,
            [FromQuery] string countryCode = "IN",
            [FromQuery] bool activeOnly = true)
        {
            try
            {
                var states = await _shippingService.GetStates(tenantId, countryCode, activeOnly);

                return Ok(new ApiResult
                {
                    Data = states
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting states: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Calculate shipping charge for a single product type
        /// </summary>
        /// <param name="request">Calculate shipping request</param>
        /// <returns>Shipping charge response</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("calculate")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> CalculateShippingCharge([FromBody] CalculateShippingRequest request)
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

                var response = await _shippingService.CalculateShippingCharge(request);

                return Ok(new ApiResult
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error calculating shipping charge: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Calculate shipping charge for mixed cart (seeds + plants)
        /// </summary>
        /// <param name="request">Calculate mixed shipping request</param>
        /// <returns>Shipping charge response</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("calculate-mixed")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> CalculateMixedShipping([FromBody] CalculateMixedShippingRequest request)
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

                var response = await _shippingService.CalculateMixedShipping(request);

                return Ok(new ApiResult
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error calculating mixed shipping: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        #endregion

        #region Admin: Shipping Rates CRUD

        /// <summary>
        /// Get all shipping rate rows for a tenant.
        /// </summary>
        [HttpGet]
        [Route("rates")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetShippingRates([FromQuery] int tenantId)
        {
            try
            {
                var rates = await _shippingService.GetShippingRates(tenantId);
                return Ok(new ApiResult { Data = rates });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting shipping rates: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Insert (ShippingRateId = 0) or update an existing shipping rate row.
        /// </summary>
        [HttpPost]
        [Route("rates")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UpsertShippingRate([FromBody] UpsertShippingRateRequest request)
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

                var rate = await _shippingService.UpsertShippingRate(request);
                return Ok(new ApiResult { Data = rate });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid shipping rate: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error upserting shipping rate: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete a shipping rate row.
        /// </summary>
        [HttpDelete]
        [Route("rates/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> DeleteShippingRate([FromRoute] long id, [FromQuery] int tenantId)
        {
            try
            {
                var rowsAffected = await _shippingService.DeleteShippingRate(id, tenantId);
                return Ok(new ApiResult { Data = new { rowsAffected } });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting shipping rate: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Activate / deactivate a shipping rate row.
        /// </summary>
        [HttpPatch]
        [Route("rates/{id}/active")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> SetShippingRateActive([FromRoute] long id, [FromQuery] int tenantId, [FromQuery] bool active)
        {
            try
            {
                var rowsAffected = await _shippingService.SetShippingRateActive(id, tenantId, active);
                return Ok(new ApiResult { Data = new { rowsAffected, active } });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error setting shipping rate active state: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        #endregion
    }
}

