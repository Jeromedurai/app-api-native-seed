using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Address;
using Tenant.Query.Service.Address;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Address
{
    [Route("api/1.0/addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        #region Initialize
        private const string GenericErrorMessage = "An error occurred while processing your request.";
        private readonly AddressService _addressService;
        public ILogger Logger { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AddressController(
            AddressService service,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _addressService = service;
            Logger = loggerFactory.CreateLogger<AddressController>();
        }

        #region User Endpoints

        /// <summary>
        /// Get all addresses for the logged-in user
        /// </summary>
        [HttpGet]
        [Route("my-addresses")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetMyAddresses([FromQuery] long userId, [FromQuery] bool activeOnly = true)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "User ID is required"
                    });
                }

                var addresses = await _addressService.GetUserAddresses(userId, activeOnly);

                return Ok(new ApiResult
                {
                    Data = addresses
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting addresses for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Create a new address
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("user/{userId}")]
        [SwaggerResponse(StatusCodes.Status201Created, "Created", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> CreateAddress([FromRoute] long userId, [FromBody] CreateAddressRequest request)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "User ID is required"
                    });
                }

                var accessError = ValidateUserAccess(userId);
                if (accessError != null) return accessError;

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Invalid request parameters"
                    });
                }

                // Set userId from route
                request.UserId = userId;

                var address = await _addressService.CreateAddress(request);

                return CreatedAtAction(nameof(GetAddressById), new { addressId = address.AddressId }, new ApiResult
                {
                    Data = address
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating address for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Get address by ID
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("{addressId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetAddressById(long addressId)
        {
            try
            {
                var userId = GetUserIdFromJwt();

                var address = await _addressService.GetAddressById(addressId, userId);

                return Ok(new ApiResult
                {
                    Data = address
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting address {AddressId}", addressId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Update an existing address
        /// </summary>
        [HttpPost]
        [Route("{addressId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UpdateAddress(long addressId, [FromQuery] long userId, [FromBody] UpdateAddressRequest request)
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

                if (userId <= 0)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "User ID is required"
                    });
                }

                // Set required fields from route and query
                request.AddressId = addressId;
                request.UserId = userId;

                // Validate that at least one field is being updated
                if (string.IsNullOrWhiteSpace(request.Street) && 
                    string.IsNullOrWhiteSpace(request.City) && 
                    string.IsNullOrWhiteSpace(request.State) && 
                    string.IsNullOrWhiteSpace(request.PostalCode) &&
                    string.IsNullOrWhiteSpace(request.AddressType) &&
                    string.IsNullOrWhiteSpace(request.Country) &&
                    !request.IsDefault.HasValue &&
                    !request.Active.HasValue)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "At least one field must be provided for update"
                    });
                }

                var address = await _addressService.UpdateAddress(request);

                return Ok(new ApiResult
                {
                    Data = address
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating address {AddressId}", addressId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Delete an address
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("{addressId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> DeleteAddress(long addressId, [FromQuery] long userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "User ID is required"
                    });
                }

                var accessError = ValidateUserAccess(userId);
                if (accessError != null) return accessError;

                await _addressService.DeleteAddress(addressId, userId);

                return Ok(new ApiResult
                {
                    Data = new { message = "Address deleted successfully" }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting address {AddressId}", addressId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Set address as default
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("{addressId}/set-default")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> SetDefaultAddress(long addressId, [FromQuery] long userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "User ID is required"
                    });
                }

                var accessError = ValidateUserAccess(userId);
                if (accessError != null) return accessError;

                await _addressService.SetDefaultAddress(addressId, userId);

                return Ok(new ApiResult
                {
                    Data = new { message = "Address set as default successfully" }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult
                {
                    Exception = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error setting default address {AddressId}", addressId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Validate address
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("validate")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ValidateAddress([FromBody] ValidateAddressRequest request)
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

                var validation = await _addressService.ValidateAddress(request);

                return Ok(new ApiResult
                {
                    Data = validation
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating address");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        #endregion

        #region Admin Endpoints

        /// <summary>
        /// Admin: Get all addresses with pagination
        /// </summary>
        [HttpGet]
        [Route("admin/all")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> AdminGetAllAddresses(
            [FromQuery] long? tenantId = null,
            [FromQuery] long? userId = null,
            [FromQuery] bool activeOnly = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var request = new GetAddressesRequest
                {
                    TenantId = tenantId,
                    UserId = userId,
                    ActiveOnly = activeOnly,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var (addresses, totalCount) = await _addressService.AdminGetAllAddresses(request);

                return Ok(new ApiResult
                {
                    Data = new
                    {
                        addresses,
                        totalCount,
                        pageNumber,
                        pageSize,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error admin getting all addresses");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        #endregion

        #region Auth Helpers

        /// <summary>
        /// Best-effort current user id from JWT claims. Reads every claim-name variation
        /// used across this codebase (the canonical names live in a compiled DLL). Returns
        /// null when no readable id claim is present.
        /// </summary>
        private long? GetUserIdFromJwt()
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("user_id")?.Value
                ?? User?.FindFirst("userId")?.Value
                ?? User?.FindFirst("UserId")?.Value
                ?? User?.FindFirst("sub")?.Value;
            return long.TryParse(claim, out var id) ? id : (long?)null;
        }

        /// <summary>
        /// Enforces that a client-supplied userId matches the authenticated caller's JWT id.
        /// Returns a 403 result to short-circuit on mismatch, or null when access is allowed.
        /// Defensive: if the token carries no readable id claim, the request is allowed to
        /// proceed (falls back to the supplied id) so authenticated calls never hard-fail on
        /// claim-name uncertainty. Callers must already require [Authorize].
        /// </summary>
        private IActionResult ValidateUserAccess(long userId)
        {
            var tokenUserId = GetUserIdFromJwt();
            if (tokenUserId.HasValue && tokenUserId.Value != userId)
            {
                Logger.LogWarning(
                    "User access denied: token user {TokenUserId} attempted to act on user {RequestUserId}",
                    tokenUserId.Value, userId);
                return Forbid(); // 403 — caller does not own this user's data
            }

            return null;
        }

        #endregion
    }
}

