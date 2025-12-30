using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tenant.API.Base.Model.Security;
using Tenant.API.Base.Repository;
using Tenant.API.Base.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using Tenant.API.Base;
using Tenant.Query.Model.Authentication;
using Newtonsoft.Json;

namespace Tenant.Query.Service.Authentication
{
    public class AuthenticationService : TnBaseService
    {
        #region Variables

        private Repository.Authentication.AuthenticationRepository _authenticationRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XtraChef.API.Authentication.Services.Authentication"/> class.
        /// </summary>
        /// <param name="authenticationRepository">Tenant repository.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public AuthenticationService(Repository.Authentication.AuthenticationRepository authenticationRepository, ILoggerFactory loggerFactory, Tenant.API.Base.Repository.TnAudit xcAudit, Tenant.API.Base.Repository.TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            _authenticationRepository = authenticationRepository;
            _authenticationRepository.Logger = loggerFactory.CreateLogger<Repository.Authentication.AuthenticationRepository>();
        }

        /// <summary>
        /// Generate tenant token
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public string GenerateTenantToken(ValidationContext validationContext)
        {
            var claims = new[] {
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_USER_ID, validationContext.UserId),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_USER_ROLE, validationContext.UserRole),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_TENANT_ID, validationContext.TenantId),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_LOCATION_ID, validationContext.LocationId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Jwt:Key"]))
            {
                KeyId = "your-key-id" // Set your key ID here
            };
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(this.Configuration["Jwt:Issuer"],
                                             this.Configuration["Jwt:Issuer"],
                                             claims,
                                             expires: DateTime.Now.AddMinutes(Double.Parse(this.Configuration["Jwt:Expiration"])),
                                             signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /// <summary>
        /// Generate tenant token
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public string CustomTenantToken(ValidationContext validationContext)
        {
            var claims = new[] {
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_USER_ID, validationContext.UserId),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_USER_ROLE, validationContext.UserRole),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_TENANT_ID, validationContext.TenantId),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_LOCATION_ID, validationContext.LocationId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Jwt:Key"]))
            {
                KeyId = "your-key-id" // Set your key ID here
            };
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(this.Configuration["Jwt:Issuer"],
                                             this.Configuration["Jwt:Issuer"],
                                             claims,
                                             expires: DateTime.Now.AddMinutes(Double.Parse(this.Configuration["Jwt:Expiration"])),
                                             signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /// <summary>
        /// Generate tenant token with expiration
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns>TokenResponse with token and expiration</returns>
        public Model.Authentication.TokenResponse CustomTenantTokenWithExpiration(ValidationContext validationContext)
        {
            var expirationMinutes = Double.Parse(this.Configuration["Jwt:Expiration"]);
            var expirationTime = DateTime.Now.AddMinutes(expirationMinutes);

            var claims = new[] {
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_USER_ID, validationContext.UserId),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_USER_ROLE, validationContext.UserRole),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_TENANT_ID, validationContext.TenantId),
                new Claim(API.Base.Core.Constant.Jwt.CLAIM_LOCATION_ID, validationContext.LocationId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Jwt:Key"]))
            {
                KeyId = "your-key-id" // Set your key ID here
            };
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(this.Configuration["Jwt:Issuer"],
                                             this.Configuration["Jwt:Issuer"],
                                             claims,
                                             expires: expirationTime,
                                             signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Convert to Unix timestamp (seconds since 1970-01-01)
            var unixTimestamp = new DateTimeOffset(expirationTime).ToUnixTimeSeconds();

            return new Model.Authentication.TokenResponse
            {
                Token = tokenString,
                Exp = unixTimestamp,
                ExpiresAt = expirationTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
        }

        public void PostClientDetails(HttpRequest Request, API.Base.Model.Security.ValidationContext validationContext, string userHostIpAddress, string userAgent)
        {
            string userHostAddress = String.Empty;
            int index = !string.IsNullOrEmpty(userHostIpAddress) ? userHostIpAddress.IndexOf(":") : 0;
            if (index > 0)
            {
                userHostAddress = !string.IsNullOrEmpty(userHostIpAddress) ? userHostIpAddress.Substring(index + 1) : string.Empty;
            }
            string clientIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (Request.Headers != null && Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                clientIpAddress = clientIpAddress + "/" + Request.Headers["X-Forwarded-For"];
            }

            ClientDetail client = new ClientDetail
            {
                ClientIp = !string.IsNullOrEmpty(userHostAddress) ? $"{clientIpAddress}/{userHostAddress}" : clientIpAddress,
                UserAgent = userAgent != null ? userAgent : "",
                Type = Request.Method,
                Endpoint = $"{Request.Host.Value}{Request.Path.Value}",
                UserId = validationContext.UserId,
                TenantId = validationContext.TenantId,
                UserRole = this.GetUserRole(validationContext.UserRole),
                LocationId = validationContext.LocationId
            };

            string json = JsonConvert.SerializeObject(client);

            this.Logger.LogInformation($"{json}");
        }

        public void CustomPostClientDetails(HttpRequest Request, API.Base.Model.Security.ValidationContext validationContext, string userHostIpAddress, string userAgent)
        {
            string userHostAddress = String.Empty;
            int index = !string.IsNullOrEmpty(userHostIpAddress) ? userHostIpAddress.IndexOf(":") : 0;
            if (index > 0)
            {
                userHostAddress = !string.IsNullOrEmpty(userHostIpAddress) ? userHostIpAddress.Substring(index + 1) : string.Empty;
            }
            string clientIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (Request.Headers != null && Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                clientIpAddress = clientIpAddress + "/" + Request.Headers["X-Forwarded-For"];
            }

            ClientDetail client = new ClientDetail
            {
                ClientIp = !string.IsNullOrEmpty(userHostAddress) ? $"{clientIpAddress}/{userHostAddress}" : clientIpAddress,
                UserAgent = userAgent != null ? userAgent : "",
                Type = Request.Method,
                Endpoint = $"{Request.Host.Value}{Request.Path.Value}",
                UserId = validationContext.UserId,
                TenantId = validationContext.TenantId,
                UserRole = this.GetUserRole(validationContext.UserRole),
                LocationId = validationContext.LocationId
            };

            string json = JsonConvert.SerializeObject(client);

            this.Logger.LogInformation($"{json}");
        }

        internal void ValidateTokenContext(ValidationContext validationContext)
        {
            try
            {
                //logger
                this.Logger.LogInformation($"Validate TenantId,LocationId,UserId and UserRole in token parameters");

                //skip the validation if the internal flag is true
                //NOTE: If it is needed to validate the TenantId,LocationId,UserId and UserRole,Remove the internal flag and give proper context.
                if (validationContext.Internal == false)
                {
                    //local variable
                    bool isInternalTenant = false;

                    //list of internal tenants
                    var internalTenants = this.Configuration["xtraCHEF:InternalTenants"];

                    //if tenant is one of the internal tenant
                    if (internalTenants != null)
                    {
                        if (internalTenants.Contains(validationContext.TenantId))
                            isInternalTenant = true;
                    }

                    if (!isInternalTenant)
                    {
                        //get spName
                        string spName = "[dbo].[XC_VALIDATE_TOKEN_CONTEXT_TEST]";

                        //null check
                        if (string.IsNullOrEmpty(spName))
                        {
                            //logger
                            this.Logger.LogWarning($"Stored procedure is not configured for ValidateTokenContext");
                            throw new KeyNotFoundException($"Stored procedure is not configured for ValidateTokenContext");
                        }

                        //validate the TenantId,LocationId,UserId and UserRole
                        bool isValid = this._authenticationRepository.ValidateTokenContext(validationContext.TenantId, validationContext.LocationId, validationContext.UserId, validationContext.UserRole, spName);

                        if (!isValid)
                        {
                            //logger
                            this.Logger.LogWarning($"Input is invalid");
                            throw new KeyNotFoundException($"Input is invalid");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private string GetUserRole(string roleId)
        {
            string role = string.Empty;

            return roleId switch
            {
                "1" => API.Base.Core.Enum.User.Role.SystemAdmin.ToString(),
                "2" => API.Base.Core.Enum.User.Role.TenantAdmin.ToString(),
                "3" => API.Base.Core.Enum.User.Role.Executive.ToString(),
                "4" => API.Base.Core.Enum.User.Role.DataEntryOperator.ToString(),
                "5" => API.Base.Core.Enum.User.Role.RestaurantManager.ToString(),
                "6" => API.Base.Core.Enum.User.Role.InvoiceCaputre.ToString(),
                "8" => API.Base.Core.Enum.User.Role.RestaurantUser.ToString(),
                _ => roleId,
            };
        }

        #endregion
    }
}
