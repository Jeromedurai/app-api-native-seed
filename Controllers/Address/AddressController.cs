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
                Logger.LogError($"Error getting user addresses: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error creating address: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
                long? userId = null;
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long parsedUserId))
                {
                    userId = parsedUserId;
                }

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
                Logger.LogError($"Error getting address by ID: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error updating address: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error deleting address: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error setting default address: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error validating address: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error admin getting all addresses: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        #endregion
    }
}

