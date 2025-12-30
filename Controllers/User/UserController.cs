using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tenant.API.Base.Attribute;
using Tenant.API.Base.Controller;
using Tenant.API.Base.Model;
using Tenant.Query.Model.User;
using Tenant.Query.Model.Admin;
using Tenant.Query.Service;

namespace Tenant.Query.Controllers
{
    [Route("api/user")]
    public class UserController : TnBaseController<Service.User.UserService>
    {
        public UserController(Service.User.UserService service, IConfiguration configuration, ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {

        }

        #region Get

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="tenantId">Tenant identifier.</param>        
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(Model.User.User))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ApiResult))]
        [Route("tenants/{userId:long}")]
        public IActionResult GetUser([FromRoute] long userId)
        {
            try
            {
                //Getting user information 
                List<Model.User.UserDetails> users = this.Service.GetUserWithAddress(userId);

                //return 
                if (users != null)
                    return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = users });
                else
                    return StatusCode(StatusCodes.Status404NotFound, new ApiResult() { Data = $"User does not exists with id '{userId}'" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        [HttpGet]
        [Route("tenants/{tenantId}/user-role")]
        [SwaggerResponse(StatusCodes.Status200OK,"", typeof(List<Model.User.Role>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError,"", typeof(ApiResult))]
        public async Task<IActionResult> GetRoles([FromRoute] string tenantId, [FromQuery] string[] role)
        {
            try
            {
                //Getting roles 
                List<Model.User.Role> roles = await this.Service.GetRoles(role);

                //return 
                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = roles });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Authenticate user login
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>User information and authentication token</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/login")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status423Locked, "Account Locked", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> Login([FromBody] Model.User.LoginRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid request model" });
                }

                var loginResponse = await this.Service.Login(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = loginResponse });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Account locked
                return StatusCode(StatusCodes.Status423Locked, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>User information and authentication token</returns>
        [HttpPost]
        [Route("auth/register")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Email or phone already exists", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> Register([FromBody] Model.User.RegisterRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var registerResponse = await this.Service.Register(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = registerResponse });
            }
            catch (InvalidOperationException ex)
            {
                // Email or phone already exists
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Logout user and invalidate tokens
        /// </summary>
        /// <param name="request">Logout details</param>
        /// <returns>Logout confirmation</returns>
        [HttpPost]
        [Route("auth/logout")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> Logout([FromBody] Model.User.LogoutRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.Logout(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get user profile information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Optional tenant ID for validation</param>
        /// <returns>User profile data</returns>
        [HttpGet]
        [Route("auth/profile")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(Model.User.UserProfileData))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetUserProfile([FromQuery] long userId, [FromQuery] long? tenantId = null)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                }

                var profileResponse = await this.Service.GetUserProfile(userId, tenantId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = profileResponse });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        /// <param name="request">Profile update details</param>
        /// <returns>Success confirmation</returns>
        [HttpPost]
        [Route("auth/update-profile")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Phone number already exists", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UpdateProfile([FromBody] Model.User.UpdateProfileRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var result = await this.Service.UpdateProfile(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Phone number already exists for another user
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Reset user password using reset token
        /// </summary>
        /// <param name="request">Password reset details</param>
        /// <returns>Success confirmation</returns>
        [HttpPost]
        [Route("auth/reset-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or expired token", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Token already used", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ResetPassword([FromBody] Model.User.ResetPasswordRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.ResetPassword(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Invalid or expired token
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Token already used or other operation conflicts
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get all users with pagination and filtering (Admin only)
        /// </summary>
        /// <param name="request">Get all users request with filters and pagination</param>
        /// <returns>Paginated list of users</returns>
        [HttpPost]
        [Route("admin/users")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetAllUsers([FromBody] Model.Admin.GetAllUsersRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate pagination parameters
                if (request.Page < 1)
                {
                    request.Page = 1;
                }

                if (request.Limit < 1 || request.Limit > 100)
                {
                    request.Limit = 10;
                }

                var result = await this.Service.GetAllUsers(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update user status (activate/deactivate) (Admin only)
        /// </summary>
        /// <param name="request">Update user status request</param>
        /// <returns>Success confirmation</returns>
        [HttpPost]
        [Route("admin/users/update-status")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UpdateUserStatus([FromBody] Model.Admin.UpdateUserStatusRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.UpdateUserStatus(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }
        #endregion
    }
}