using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tenant.Query.Model.Contact;
using Tenant.Query.Service.Contact;

namespace Tenant.Query.Controllers.Contact
{
    [Route("api/1.0/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;
        public ILogger Logger { get; set; }

        public ContactController(
            ContactService contactService,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _contactService = contactService;
            Logger = loggerFactory.CreateLogger<ContactController>();
        }

        /// <summary>
        /// Public endpoint to submit a contact-us message.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("messages")]
        [SwaggerResponse(StatusCodes.Status200OK, "Message accepted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public async Task<IActionResult> SubmitContactMessage([FromBody] ContactMessageRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request payload is required" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid contact form data", errors = ModelState });
                }

                var id = await _contactService.CreateContactMessage(request);

                return Ok(new
                {
                    message = "Thank you for contacting us. We will get back to you soon.",
                    contactId = id
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error submitting contact message");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while submitting your message. Please try again later.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Admin endpoint to list submitted contact-us messages.
        /// </summary>
        /// <param name="tenantId">Optional tenant scope. When omitted, returns all tenants.</param>
        [Authorize]
        [HttpGet]
        [Route("messages")]
        [SwaggerResponse(StatusCodes.Status200OK, "Messages retrieved")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public async Task<IActionResult> GetContactMessages([FromQuery] long? tenantId = null)
        {
            try
            {
                var messages = await _contactService.GetContactMessages(tenantId);

                return Ok(new
                {
                    count = messages.Count,
                    messages
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving contact messages");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving contact messages.",
                    error = ex.Message
                });
            }
        }
    }
}

