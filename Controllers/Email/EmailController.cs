using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Email;
using Tenant.Query.Service.Email;

namespace Tenant.Query.Controllers.Email
{
    /// <summary>
    /// Email Controller for sending emails through API endpoints
    /// </summary>
    [ApiController]
    [Route("api/1.0/email")]
    [SwaggerTag("Email Management")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        public ILogger Logger { get; set; }

        public EmailController(
            EmailService emailService,
            ILoggerFactory loggerFactory)
        {
            _emailService = emailService;
            this.Logger = loggerFactory.CreateLogger<EmailController>();
        }

        /// <summary>
        /// Send email using a template
        /// </summary>
        /// <param name="request">Email request with template name, recipient, subject, and template data</param>
        /// <returns>Email response indicating success or failure</returns>
        [HttpPost("send")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Email request is required" });
                }

                if (string.IsNullOrWhiteSpace(request.To))
                {
                    return BadRequest(new ApiResult { Exception = "Recipient email address (To) is required" });
                }

                if (string.IsNullOrWhiteSpace(request.TemplateName))
                {
                    return BadRequest(new ApiResult { Exception = "Template name is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Subject))
                {
                    return BadRequest(new ApiResult { Exception = "Email subject is required" });
                }

                var response = await _emailService.SendEmail(request);

                if (response.Success)
                {
                    return Ok(new ApiResult { Data = response });
                }
                else
                {
                    return BadRequest(new ApiResult { Exception = response.Message });
                }
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid email request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error sending email: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while sending the email." });
            }
        }
    }
}
