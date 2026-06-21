using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                    message = "An error occurred while submitting your message. Please try again later."
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
            if (tenantId.HasValue && tenantId.Value <= 0)
            {
                return BadRequest(new { message = "tenantId must be a positive number." });
            }

            var accessError = ValidateTenantAccess(tenantId);
            if (accessError != null) return accessError;

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
                    message = "An error occurred while retrieving contact messages."
                });
            }
        }

        /// <summary>
        /// Admin endpoint to mark a contact-us message as viewed (read).
        /// </summary>
        /// <param name="id">Contact message id.</param>
        /// <param name="viewedBy">Optional admin user id who viewed it.</param>
        /// <param name="tenantId">Optional tenant scope.</param>
        [Authorize]
        [HttpPatch]
        [Route("messages/{id:long}/viewed")]
        [SwaggerResponse(StatusCodes.Status200OK, "Message marked viewed")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public async Task<IActionResult> MarkContactMessageViewed(
            long id,
            [FromQuery] long? viewedBy = null,
            [FromQuery] long? tenantId = null)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "A valid message id is required." });
            }

            if (tenantId.HasValue && tenantId.Value <= 0)
            {
                return BadRequest(new { message = "tenantId must be a positive number." });
            }

            var accessError = ValidateTenantAccess(tenantId);
            if (accessError != null) return accessError;

            try
            {
                var rowsAffected = await _contactService.MarkContactMessageViewed(id, viewedBy, tenantId);

                return Ok(new
                {
                    id,
                    isViewed = true,
                    updated = rowsAffected > 0
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error marking contact message {Id} viewed", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while updating the message."
                });
            }
        }

        /// <summary>
        /// Best-effort tenant id from JWT claims. Reads the claim-name variations used across
        /// this codebase (canonical names live in a compiled DLL). Null when no readable claim.
        /// </summary>
        private long? GetTenantIdFromJwt()
        {
            var claim = User?.FindFirst("tenant_id")?.Value
                ?? User?.FindFirst("tenantId")?.Value;
            return long.TryParse(claim, out var id) ? id : (long?)null;
        }

        /// <summary>
        /// Enforces that a client-supplied tenantId does not contradict the caller's JWT tenant.
        /// Returns a 403 result on mismatch, or null when access is allowed. Defensive: when the
        /// client omits tenantId, or the token has no readable tenant claim, the request proceeds
        /// unchanged (preserves existing behavior). Callers must already require [Authorize].
        /// </summary>
        private IActionResult ValidateTenantAccess(long? tenantId)
        {
            var tokenTenantId = GetTenantIdFromJwt();
            if (tenantId.HasValue && tokenTenantId.HasValue && tokenTenantId.Value != tenantId.Value)
            {
                Logger.LogWarning(
                    "Tenant access denied: token tenant {TokenTenantId} attempted to act on tenant {RequestTenantId}",
                    tokenTenantId.Value, tenantId.Value);
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have access to this tenant." });
            }

            return null;
        }
    }
}

