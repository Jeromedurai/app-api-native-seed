
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Controller;

using Tenant.API.Base.Model.Security;
using Tenant.Query.Service.Authentication;

namespace Tenant.Query.Controllers.Authentication
{
    [Route("api/[controller]")]
    public class AuthenticationController : TnBaseController<Service.Authentication.AuthenticationService>
    {
        public AuthenticationController(AuthenticationService service, IConfiguration configuration, ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {   
        }

        /// <summary>
        /// Generate the tenant token.
        /// </summary>
        /// <returns>The token.</returns>
        /// <param name="validatio
        /// nContext">Validation context.</param>
        [AllowAnonymous]
        [HttpPost]      
        [Route("token")]
        [Produces("text/plain")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GenerateTenantToken([FromBody] ValidationContext validationContext, [FromQuery] string userHostIpAddress = null, [FromQuery] string userAgent = null)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //validate the token context
                    this.Service.ValidateTokenContext(validationContext);

                    //string message;
                    Logger.LogInformation($"Valid token parameters: {validationContext}");

                    //generate a token
                    string token = this.Service.GenerateTenantToken(validationContext);

                    this.Service.PostClientDetails(Request, validationContext, userHostIpAddress, userAgent);

                    //return token
                    return StatusCode(StatusCodes.Status200OK, token);

                }
                else
                {
                    //return error
                    BadRequestObjectResult badObjectResult = new BadRequestObjectResult(ModelState);
                    this.Logger.LogError(badObjectResult.ToString());

                    return StatusCode(StatusCodes.Status400BadRequest, badObjectResult);
                }
            }
            catch (KeyNotFoundException ex)
            {
                //return error
                BadRequestObjectResult badObjectResult = new BadRequestObjectResult(ex.Message);

                return StatusCode(StatusCodes.Status400BadRequest, badObjectResult);
            }
            catch (System.Exception ex)
            {
                /*BadRequestObjectResult badObjectResult = new BadRequestObjectResult(ModelState);*/
                this.Logger.LogError($"error in generating token ({ex.Message}): {ex.InnerException}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Generate the tenant token with expiration.
        /// </summary>
        /// <returns>The token with expiration details.</returns>
        /// <param name="validatio
        /// nContext">Validation context.</param>
        [AllowAnonymous]
        [HttpPost]      
        [Route("tenant-token")]
        [Produces("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CustomTenantToken([FromBody] ValidationContext validationContext, [FromQuery] string userHostIpAddress = null, [FromQuery] string userAgent = null)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //validate the token context
                    this.Service.ValidateTokenContext(validationContext);

                    //string message;
                    Logger.LogInformation($"Valid token parameters: {validationContext}");

                    //generate a token with expiration
                    var tokenResponse = this.Service.CustomTenantTokenWithExpiration(validationContext);

                    this.Service.CustomPostClientDetails(Request, validationContext, userHostIpAddress, userAgent);

                    //return token response
                    return StatusCode(StatusCodes.Status200OK, tokenResponse);

                }
                else
                {
                    //return error
                    BadRequestObjectResult badObjectResult = new BadRequestObjectResult(ModelState);
                    this.Logger.LogError(badObjectResult.ToString());

                    return StatusCode(StatusCodes.Status400BadRequest, badObjectResult);
                }
            }
            catch (KeyNotFoundException ex)
            {
                //return error
                BadRequestObjectResult badObjectResult = new BadRequestObjectResult(ex.Message);

                return StatusCode(StatusCodes.Status400BadRequest, badObjectResult);
            }
            catch (System.Exception ex)
            {
                /*BadRequestObjectResult badObjectResult = new BadRequestObjectResult(ModelState);*/
                this.Logger.LogError($"error in generating token ({ex.Message}): {ex.InnerException}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
