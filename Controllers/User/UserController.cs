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
using Tenant.Query.Service.Authentication;
using Tenant.Query.Model.Authentication;
using System.Security.Cryptography;
using Google.Apis.Auth;

namespace Tenant.Query.Controllers
{
    [Route("api/user")]
    public class UserController : TnBaseController<Service.User.UserService>
    {
        private readonly ILoggerFactory _loggerFactory;

        public UserController(Service.User.UserService service, IConfiguration configuration, ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {
            _loggerFactory = loggerFactory;
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
            catch (InvalidOperationException ex) when (ex.Message.Contains("not enabled") || ex.Message.Contains("Only mobile OTP"))
            {
                Logger.LogWarning($"Password login disabled: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
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
        /// Get encryption key for client-side encryption
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("encryption/key")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public IActionResult GetEncryptionKey()
        {
            try
            {
                var encryptionService = new EncryptionService(Configuration, _loggerFactory.CreateLogger<EncryptionService>());
                var (key, nonce) = encryptionService.GenerateEncryptionKey();
                
                return Ok(new ApiResult
                {
                    Data = new EncryptionKeyResponse
                    {
                        Key = key,
                        Nonce = nonce,
                        ExpiresIn = 300 // 5 minutes
                    }
                });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error generating encryption key: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Login with encrypted credentials
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/login-encrypted")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status423Locked, "Account Locked", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> LoginEncrypted([FromBody] EncryptedLoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Ciphertext) || 
                    string.IsNullOrEmpty(request.Nonce) || string.IsNullOrEmpty(request.Tag))
                {
                    return BadRequest(new ApiResult { Exception = "Invalid encrypted request" });
                }

                var encryptionService = new EncryptionService(Configuration, _loggerFactory.CreateLogger<EncryptionService>());
                var loginRequest = encryptionService.DecryptLoginRequest(request.Ciphertext, request.Nonce, request.Tag);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid request model" });
                }

                var loginResponse = await this.Service.Login(loginRequest);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = loginResponse });
            }
            catch (CryptographicException ex)
            {
                Logger.LogWarning($"Decryption failed: {ex.Message}");
                return BadRequest(new ApiResult { Exception = "Invalid encrypted payload" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not enabled") || ex.Message.Contains("Only mobile OTP"))
            {
                Logger.LogWarning($"Password login disabled: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status423Locked, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Login error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset OTP
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/forgot-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ForgotPassword([FromBody] Model.User.ForgotPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new ApiResult { Exception = "Email is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await this.Service.RequestPasswordReset(request, ipAddress, userAgent);

                return Ok(new ApiResult { Data = response });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Forgot password error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Reset password with OTP
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/reset-password-with-otp")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] Model.User.ResetPasswordWithOtpRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await this.Service.ResetPasswordWithOtp(request, ipAddress, userAgent);

                return Ok(new ApiResult { Data = response });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Reset password error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Resend password reset OTP
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/resend-reset-otp")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ResendResetOtp([FromBody] Model.User.ForgotPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new ApiResult { Exception = "Email is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await this.Service.ResendResetOtp(request, ipAddress, userAgent);

                return Ok(new ApiResult { Data = response });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Resend OTP error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Request login OTP for mobile authentication
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/request-login-otp")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> RequestLoginOtp([FromBody] Model.User.RequestLoginOtpRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Phone))
                {
                    return BadRequest(new ApiResult { Exception = "Phone number is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await this.Service.RequestLoginOtp(request, ipAddress, userAgent);
                response.OTP = null;

                return Ok(new ApiResult { Data = response });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Unable to send OTP"))
            {
                Logger.LogWarning($"Login OTP SMS failed: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not enabled"))
            {
                Logger.LogWarning($"OTP login disabled: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Request login OTP error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Login with mobile OTP
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/login-with-otp")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid OTP", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> LoginWithOtp([FromBody] Model.User.LoginWithOtpRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await this.Service.LoginWithOtp(request, ipAddress, userAgent);

                return Ok(new ApiResult { Data = response });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Unable to send OTP"))
            {
                Logger.LogWarning($"Login OTP SMS failed: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not enabled"))
            {
                Logger.LogWarning($"OTP login disabled: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Login with OTP error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Resend login OTP
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/resend-login-otp")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ResendLoginOtp([FromBody] Model.User.RequestLoginOtpRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Phone))
                {
                    return BadRequest(new ApiResult { Exception = "Phone number is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await this.Service.ResendLoginOtp(request, ipAddress, userAgent);
                response.OTP = null;

                return Ok(new ApiResult { Data = response });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Unable to send OTP"))
            {
                Logger.LogWarning($"Login OTP SMS failed: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not enabled"))
            {
                Logger.LogWarning($"OTP login disabled: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Resend login OTP error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Register with encrypted credentials
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/register-encrypted")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Email or phone already exists", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> RegisterEncrypted([FromBody] EncryptedRegisterRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Ciphertext) || 
                    string.IsNullOrEmpty(request.Nonce) || string.IsNullOrEmpty(request.Tag))
                {
                    return BadRequest(new ApiResult { Exception = "Invalid encrypted request" });
                }

                var encryptionService = new EncryptionService(Configuration, _loggerFactory.CreateLogger<EncryptionService>());
                var registerRequest = encryptionService.DecryptRegisterRequest(request.Ciphertext, request.Nonce, request.Tag);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var registerResponse = await this.Service.Register(registerRequest);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = registerResponse });
            }
            catch (CryptographicException ex)
            {
                Logger.LogWarning($"Decryption failed: {ex.Message}");
                return BadRequest(new ApiResult { Exception = "Invalid encrypted payload" });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Registration error: {ex.Message}");
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
        /// Upload / update the current user's profile image (multipart/form-data).
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("auth/profile-image")]
        [RequestFormLimits(MultipartBodyLengthLimit = 5 * 1024 * 1024)]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UploadProfileImage([FromForm] Model.User.ProfileImageUploadDto dto)
        {
            if (dto == null || dto.UserId <= 0)
            {
                return BadRequest(new ApiResult { Exception = "A valid user id is required." });
            }

            if (dto.TenantId.HasValue && dto.TenantId.Value <= 0)
            {
                return BadRequest(new ApiResult { Exception = "tenantId must be a positive number." });
            }

            try
            {
                var profilePictureUrl = await this.Service.UpdateProfileImageAsync(dto);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = new { profilePictureUrl } });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResult { Exception = "User not found or inactive." });
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "Error uploading profile image for user {UserId}", dto.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResult { Exception = "An error occurred while updating the profile image." });
            }
        }

        /// <summary>
        /// Public endpoint that streams a user's profile image bytes (cacheable).
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("auth/profile-image/{userId:long}")]
        [ResponseCache(Duration = 86400)]
        public async Task<IActionResult> GetProfileImage([FromRoute] long userId, [FromQuery] long? tenantId = null)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiResult { Exception = "A valid user id is required." });
            }

            try
            {
                var image = await this.Service.GetProfileImageAsync(userId, tenantId);
                if (image?.ImageData == null || image.ImageData.Length == 0)
                {
                    return NotFound();
                }

                return File(image.ImageData, string.IsNullOrWhiteSpace(image.ContentType) ? "image/webp" : image.ContentType);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "Error reading profile image for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResult { Exception = "An error occurred while loading the profile image." });
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
        [Authorize]
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
        [Authorize]
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
        /// <summary>
        /// Google social login — verifies the Google ID token and returns a user session
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth/google")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid Google token", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GoogleLogin([FromBody] Model.User.GoogleLoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
                    return BadRequest(new ApiResult { Exception = "Google ID token is required" });

                // Verify the Google ID token
                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                }
                catch (InvalidJwtException ex)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResult { Exception = "Invalid or expired Google token: " + ex.Message });
                }

                var loginResponse = await this.Service.GoogleLogin(payload.Email, payload.GivenName ?? "", payload.FamilyName ?? "", request.TenantId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = loginResponse });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not enabled") || ex.Message.Contains("Only mobile OTP"))
            {
                Logger.LogWarning($"Google login disabled: {ex.Message}");
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }
        #endregion
    }
}