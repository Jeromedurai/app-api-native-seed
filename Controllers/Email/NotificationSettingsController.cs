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
    /// Admin CRUD for SA_EMAILTEMPLATE and SA_EMAILSCHEDULE.
    ///
    /// Tenant scope: every endpoint takes a `tenantId` query parameter. Reads
    /// return rows where TENANTID = tenantId OR TENANTID IS NULL (globals).
    /// Writes are pinned to the caller's tenant; the controller forces the
    /// payload's TenantId to match the query-string tenantId so a malicious
    /// admin cannot escape their tenant by mutating the body.
    /// Pass tenantId = null (omit the param) for super-admin / global-only
    /// access — leave that to gateway-level RBAC.
    /// </summary>
    [ApiController]
    [Route("api/1.0/email")]
    [SwaggerTag("Notification Settings (Templates + Schedules)")]
    public class NotificationSettingsController : ControllerBase
    {
        private readonly NotificationSettingsService _service;
        public ILogger Logger { get; set; }

        public NotificationSettingsController(
            NotificationSettingsService service,
            ILoggerFactory loggerFactory)
        {
            _service = service;
            this.Logger = loggerFactory.CreateLogger<NotificationSettingsController>();
        }

        // =========================================================
        // Templates
        // =========================================================

        [HttpGet("templates")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> GetTemplates([FromQuery] long? tenantId)
        {
            try
            {
                var result = await _service.GetTemplatesAsync(tenantId);
                return Ok(new ApiResult { Data = result });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error listing email templates: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to load email templates." });
            }
        }

        [HttpGet("templates/{id:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        public async Task<IActionResult> GetTemplate([FromRoute] long id, [FromQuery] long? tenantId)
        {
            try
            {
                var template = await _service.GetTemplateAsync(id, tenantId);
                if (template == null) return NotFound(new ApiResult { Exception = $"Template {id} not found." });
                return Ok(new ApiResult { Data = template });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error reading email template {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to load email template." });
            }
        }

        [HttpPost("templates")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> CreateTemplate([FromQuery] long? tenantId, [FromBody] UpsertEmailTemplateRequest request)
        {
            try
            {
                if (request != null) request.TenantId = tenantId; // pin write scope
                var created = await _service.CreateTemplateAsync(request, tenantId);
                return Ok(new ApiResult { Data = created });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error creating email template: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to create email template." });
            }
        }

        [HttpPut("templates/{id:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> UpdateTemplate([FromRoute] long id, [FromQuery] long? tenantId, [FromBody] UpsertEmailTemplateRequest request)
        {
            try
            {
                if (request != null) request.TenantId = tenantId; // pin write scope
                var updated = await _service.UpdateTemplateAsync(id, request, tenantId);
                if (updated == null) return NotFound(new ApiResult { Exception = $"Template {id} not found." });
                return Ok(new ApiResult { Data = updated });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error updating email template {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to update email template." });
            }
        }

        [HttpDelete("templates/{id:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> DeleteTemplate([FromRoute] long id, [FromQuery] long? tenantId)
        {
            try
            {
                var rows = await _service.DeleteTemplateAsync(id, tenantId);
                return Ok(new ApiResult { Data = new { rowsAffected = rows } });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error deleting email template {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to delete email template." });
            }
        }

        [HttpPatch("templates/{id:long}/active")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> SetTemplateActive([FromRoute] long id, [FromQuery] long? tenantId, [FromBody] SetActiveRequest request)
        {
            try
            {
                if (request == null) return BadRequest(new ApiResult { Exception = "Active flag is required." });
                var rows = await _service.SetTemplateActiveAsync(id, request.Active, tenantId);
                return Ok(new ApiResult { Data = new { rowsAffected = rows, active = request.Active } });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error toggling active for template {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to update template active flag." });
            }
        }

        // =========================================================
        // Schedules
        // =========================================================

        [HttpGet("schedules")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> GetSchedules([FromQuery] long? tenantId)
        {
            try
            {
                var result = await _service.GetSchedulesAsync(tenantId);
                return Ok(new ApiResult { Data = result });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error listing email schedules: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to load email schedules." });
            }
        }

        [HttpPost("schedules")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> CreateSchedule([FromQuery] long? tenantId, [FromBody] UpsertEmailScheduleRequest request)
        {
            try
            {
                if (request != null) request.TenantId = tenantId; // pin write scope
                var created = await _service.CreateScheduleAsync(request, tenantId);
                return Ok(new ApiResult { Data = created });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error creating email schedule: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to create email schedule." });
            }
        }

        [HttpPut("schedules/{id:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> UpdateSchedule([FromRoute] long id, [FromQuery] long? tenantId, [FromBody] UpsertEmailScheduleRequest request)
        {
            try
            {
                if (request != null) request.TenantId = tenantId; // pin write scope
                var updated = await _service.UpdateScheduleAsync(id, request, tenantId);
                if (updated == null) return NotFound(new ApiResult { Exception = $"Schedule {id} not found." });
                return Ok(new ApiResult { Data = updated });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error updating email schedule {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to update email schedule." });
            }
        }

        [HttpDelete("schedules/{id:long}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> DeleteSchedule([FromRoute] long id, [FromQuery] long? tenantId)
        {
            try
            {
                var rows = await _service.DeleteScheduleAsync(id, tenantId);
                return Ok(new ApiResult { Data = new { rowsAffected = rows } });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error deleting email schedule {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to delete email schedule." });
            }
        }

        [HttpPatch("schedules/{id:long}/active")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        public async Task<IActionResult> SetScheduleActive([FromRoute] long id, [FromQuery] long? tenantId, [FromBody] SetActiveRequest request)
        {
            try
            {
                if (request == null) return BadRequest(new ApiResult { Exception = "Active flag is required." });
                var rows = await _service.SetScheduleActiveAsync(id, request.Active, tenantId);
                return Ok(new ApiResult { Data = new { rowsAffected = rows, active = request.Active } });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error toggling active for schedule {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to update schedule active flag." });
            }
        }
    }
}
