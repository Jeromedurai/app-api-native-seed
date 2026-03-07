using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace Tenant.Query.Helpers
{
    /// <summary>
    /// Helper class for managing HttpOnly cookies securely
    /// Provides reusable methods for setting and reading HttpOnly cookies
    /// HttpOnly cookies prevent JavaScript access, protecting against XSS attacks
    /// 
    /// USAGE EXAMPLE:
    /// ==============
    /// 
    /// // In your controller constructor, inject IWebHostEnvironment:
    /// private readonly IWebHostEnvironment _environment;
    /// public YourController(..., IWebHostEnvironment environment)
    /// {
    ///     _environment = environment;
    /// }
    /// 
    /// // GET endpoint - Check cookies first, then DB:
    /// [HttpGet("your-endpoint")]
    /// public IActionResult GetData()
    /// {
    ///     const string COOKIE_NAME = "YourData_Cache";
    ///     
    ///     // Try reading from httpOnly cookie first
    ///     var cachedData = HttpOnlyCookieHelper.GetHttpOnlyJsonCookie<YourDataType>(Request, COOKIE_NAME);
    ///     if (cachedData != null)
    ///     {
    ///         // Refresh cookie expiration
    ///         HttpOnlyCookieHelper.SetHttpOnlyJsonCookie(Response, _environment, COOKIE_NAME, cachedData);
    ///         return Ok(new ApiResult { Data = cachedData });
    ///     }
    ///     
    ///     // Cookie not found - fetch from DB
    ///     var data = this.Service.GetData();
    ///     
    ///     // Store in httpOnly cookie
    ///     HttpOnlyCookieHelper.SetHttpOnlyJsonCookie(Response, _environment, COOKIE_NAME, data);
    ///     
    ///     return Ok(new ApiResult { Data = data });
    /// }
    /// 
    /// // POST endpoint - Update cookies after save:
    /// [HttpPost("your-endpoint")]
    /// public IActionResult SaveData([FromBody] YourRequest request)
    /// {
    ///     var result = this.Service.SaveData(request);
    ///     
    ///     if (result.Success)
    ///     {
    ///         // Update httpOnly cookie
    ///         HttpOnlyCookieHelper.SetHttpOnlyJsonCookie(Response, _environment, "YourData_Cache", result.Data);
    ///     }
    ///     
    ///     return Ok(new ApiResult { Data = result });
    /// }
    /// </summary>
    public static class HttpOnlyCookieHelper
    {
        /// <summary>
        /// Creates secure HttpOnly cookie options
        /// </summary>
        /// <param name="environment">Web host environment to determine if production</param>
        /// <param name="expirationDays">Number of days until cookie expires (default: 30)</param>
        /// <param name="path">Cookie path (default: "/")</param>
        /// <returns>CookieOptions with HttpOnly, Secure, and SameSite settings</returns>
        public static CookieOptions CreateSecureCookieOptions(
            IWebHostEnvironment environment, 
            int expirationDays = 30, 
            string path = "/")
        {
            var isProduction = environment != null && 
                              string.Equals(environment.EnvironmentName, Microsoft.Extensions.Hosting.Environments.Production, StringComparison.OrdinalIgnoreCase);
            
            return new CookieOptions
            {
                HttpOnly = true, // CRITICAL: Prevents JavaScript access - protects against XSS attacks
                Secure = isProduction, // Only sent over HTTPS in production (allows HTTP in development)
                SameSite = SameSiteMode.Lax, // CSRF protection - allows same-site requests
                Expires = DateTimeOffset.UtcNow.AddDays(expirationDays),
                Path = path // Cookie available site-wide
            };
        }

        /// <summary>
        /// Sets a single HttpOnly cookie securely
        /// </summary>
        /// <param name="response">HTTP response object</param>
        /// <param name="environment">Web host environment</param>
        /// <param name="cookieName">Name of the cookie</param>
        /// <param name="cookieValue">Value to store in cookie</param>
        /// <param name="expirationDays">Number of days until expiration (default: 30)</param>
        public static void SetHttpOnlyCookie(
            HttpResponse response,
            IWebHostEnvironment environment,
            string cookieName,
            string cookieValue,
            int expirationDays = 30)
        {
            if (response == null || string.IsNullOrEmpty(cookieName))
                return;

            var cookieOptions = CreateSecureCookieOptions(environment, expirationDays);
            response.Cookies.Append(cookieName, cookieValue ?? string.Empty, cookieOptions);
        }

        /// <summary>
        /// Sets multiple HttpOnly cookies from a dictionary
        /// </summary>
        /// <param name="response">HTTP response object</param>
        /// <param name="environment">Web host environment</param>
        /// <param name="cookies">Dictionary of cookie name-value pairs</param>
        /// <param name="expirationDays">Number of days until expiration (default: 30)</param>
        public static void SetHttpOnlyCookies(
            HttpResponse response,
            IWebHostEnvironment environment,
            Dictionary<string, string> cookies,
            int expirationDays = 30)
        {
            if (response == null || cookies == null || cookies.Count == 0)
                return;

            var cookieOptions = CreateSecureCookieOptions(environment, expirationDays);
            
            foreach (var cookie in cookies)
            {
                if (!string.IsNullOrEmpty(cookie.Key))
                {
                    response.Cookies.Append(cookie.Key, cookie.Value ?? string.Empty, cookieOptions);
                }
            }
        }

        /// <summary>
        /// Sets a JSON HttpOnly cookie (for complex objects)
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="response">HTTP response object</param>
        /// <param name="environment">Web host environment</param>
        /// <param name="cookieName">Name of the cookie</param>
        /// <param name="data">Object to serialize and store</param>
        /// <param name="expirationDays">Number of days until expiration (default: 30)</param>
        public static void SetHttpOnlyJsonCookie<T>(
            HttpResponse response,
            IWebHostEnvironment environment,
            string cookieName,
            T data,
            int expirationDays = 30)
        {
            if (response == null || string.IsNullOrEmpty(cookieName) || data == null)
                return;

            try
            {
                var jsonValue = JsonSerializer.Serialize(data);
                SetHttpOnlyCookie(response, environment, cookieName, jsonValue, expirationDays);
            }
            catch
            {
                // Log error if needed - silently fail to prevent breaking the request
            }
        }

        /// <summary>
        /// Reads a HttpOnly cookie value
        /// </summary>
        /// <param name="request">HTTP request object</param>
        /// <param name="cookieName">Name of the cookie to read</param>
        /// <returns>Cookie value or null if not found</returns>
        public static string GetHttpOnlyCookie(HttpRequest request, string cookieName)
        {
            if (request == null || string.IsNullOrEmpty(cookieName))
                return null;

            return request.Cookies[cookieName];
        }

        /// <summary>
        /// Reads and deserializes a JSON HttpOnly cookie
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="request">HTTP request object</param>
        /// <param name="cookieName">Name of the cookie to read</param>
        /// <returns>Deserialized object or default(T) if not found or invalid</returns>
        public static T GetHttpOnlyJsonCookie<T>(HttpRequest request, string cookieName) where T : class
        {
            var cookieValue = GetHttpOnlyCookie(request, cookieName);
            
            if (string.IsNullOrEmpty(cookieValue))
                return default(T);

            try
            {
                // Use JsonSerializerOptions to handle case-insensitive property names
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };
                
                return JsonSerializer.Deserialize<T>(cookieValue, options);
            }
            catch (JsonException)
            {
                // Return default if deserialization fails
                return default(T);
            }
            catch
            {
                // Return default for any other exception
                return default(T);
            }
        }

        /// <summary>
        /// Deletes a HttpOnly cookie
        /// </summary>
        /// <param name="response">HTTP response object</param>
        /// <param name="environment">Web host environment</param>
        /// <param name="cookieName">Name of the cookie to delete</param>
        public static void DeleteHttpOnlyCookie(
            HttpResponse response,
            IWebHostEnvironment environment,
            string cookieName)
        {
            if (response == null || string.IsNullOrEmpty(cookieName))
                return;

            var cookieOptions = CreateSecureCookieOptions(environment, 0);
            cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(-1); // Set expiration in the past
            response.Cookies.Append(cookieName, string.Empty, cookieOptions);
        }

        /// <summary>
        /// Checks if a HttpOnly cookie exists
        /// </summary>
        /// <param name="request">HTTP request object</param>
        /// <param name="cookieName">Name of the cookie to check</param>
        /// <returns>True if cookie exists and has a value</returns>
        public static bool HttpOnlyCookieExists(HttpRequest request, string cookieName)
        {
            if (request == null || string.IsNullOrEmpty(cookieName))
                return false;

            return request.Cookies.ContainsKey(cookieName) && 
                   !string.IsNullOrEmpty(request.Cookies[cookieName]);
        }
    }
}
