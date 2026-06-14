using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Email;
using Tenant.Query.Repository.Email;
using Tenant.Query.Service.Email;
using Tenant.Query.Service.Product;

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
        private readonly NotificationDispatcherService _dispatcher;
        private readonly NotificationWhatsAppDispatcherService _whatsAppDispatcher;
        private readonly EmailNotificationRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ProductService _productService;
        public ILogger Logger { get; set; }

        public EmailController(
            EmailService emailService,
            NotificationDispatcherService dispatcher,
            NotificationWhatsAppDispatcherService whatsAppDispatcher,
            EmailNotificationRepository repository,
            IConfiguration configuration,
            ProductService productService,
            ILoggerFactory loggerFactory)
        {
            _emailService = emailService;
            _dispatcher = dispatcher;
            _whatsAppDispatcher = whatsAppDispatcher;
            _repository = repository;
            _configuration = configuration;
            _productService = productService;
            this.Logger = loggerFactory.CreateLogger<EmailController>();
        }

        /// <summary>
        /// Shared-secret check for the worker→API dispatch endpoints (which are
        /// AllowAnonymous so the worker can reach them without a JWT). The worker
        /// sends header "X-Worker-Key"; it must match Notifications:WorkerKey.
        /// If the key is not configured, the check is skipped (dev convenience) but
        /// a warning is logged so it is not silently left open in production.
        /// </summary>
        private bool IsWorkerAuthorized()
        {
            var expected = _configuration["Notifications:WorkerKey"];
            if (string.IsNullOrWhiteSpace(expected))
            {
                Logger.LogWarning("Notifications:WorkerKey is not set — dispatch endpoints are UNAUTHENTICATED.");
                return true;
            }
            var provided = Request.Headers["X-Worker-Key"].ToString();
            return string.Equals(provided, expected, System.StringComparison.Ordinal);
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

        /// <summary>
        /// Dispatch endpoint consumed by the AppNativeNotification Windows service.
        /// Accepts a queue row payload (TEMPLATEID + EMAILSP), resolves the Razor view from
        /// SA_EMAILTEMPLATE, parses EMAILSP (static "Email~Name~Body" or dynamic "SP_NAME|params"),
        /// renders, and sends. Returns 200 on success / 4xx on failure so the worker's
        /// existing retry logic in NotificationProcessor.SendOneAsync continues to drive retries.
        /// </summary>
        [HttpPost("notification/send")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> DispatchQueuedEmail([FromBody] QueuedEmailDispatchRequest request)
        {
            try
            {
                if (!IsWorkerAuthorized())
                {
                    return Unauthorized(new ApiResult { Exception = "Invalid or missing worker key." });
                }
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Dispatch request is required" });
                }
                if (request.TemplateId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "TemplateId is required" });
                }
                if (request.TenantId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "TenantId is required" });
                }
                if (string.IsNullOrWhiteSpace(request.EmailSP))
                {
                    return BadRequest(new ApiResult { Exception = "EmailSP payload is required" });
                }

                // Channel feature flag (AppSettings → appsettings.json → default ON).
                if (!bool.TryParse(_configuration["Notifications:Email:Enabled"], out var jsonEmailOn))
                {
                    jsonEmailOn = true; // default ON when not in config
                }
                var emailOn = _productService.GetSettingBool("EMAIL_ENABLED", jsonEmailOn);
                if (!emailOn)
                {
                    return Ok(new ApiResult { Data = new SendEmailResponse { Success = true, Skipped = true, Message = "Email channel is disabled." } });
                }

                var response = await _dispatcher.DispatchAsync(request);

                // Skipped (e.g. template inactive) is reported as success so the
                // worker marks the queue row as done and stops retrying.
                if (response.Success)
                {
                    return Ok(new ApiResult { Data = response });
                }

                return BadRequest(new ApiResult { Exception = response.Message });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Invalid dispatch request: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error dispatching queued email: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while dispatching the queued email." });
            }
        
        }

        /// <summary>
        /// Dispatch a queued WhatsApp message (worker posts SA_EMAILMASTER rows with CHANNEL = WhatsApp).
        /// Gated by Notifications:WhatsApp:Enabled (default off): disabled → 200 Skipped (no send, no retry).
        /// </summary>
        [HttpPost("notification/whatsapp")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> DispatchQueuedWhatsApp([FromBody] QueuedEmailDispatchRequest request)
        {
            try
            {
                if (!IsWorkerAuthorized())
                {
                    return Unauthorized(new ApiResult { Exception = "Invalid or missing worker key." });
                }
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Dispatch request is required" });
                }
                if (request.TemplateId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "TemplateId is required" });
                }
                if (request.TenantId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "TenantId is required" });
                }
                if (string.IsNullOrWhiteSpace(request.EmailSP))
                {
                    return BadRequest(new ApiResult { Exception = "EmailSP payload is required" });
                }

                var response = await _whatsAppDispatcher.DispatchAsync(request);

                if (response.Success)
                {
                    return Ok(new ApiResult { Data = response });
                }
                return BadRequest(new ApiResult { Exception = response.Message });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error dispatching queued WhatsApp: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while dispatching the queued WhatsApp message." });
            }
        }

        /// <summary>
        /// Diagnostic banner data for the React admin UI: server timezone, current
        /// server time, the worker's last poll, and the polling cadence (read from
        /// the API's appsettings; the worker itself owns the live value but we
        /// mirror the configured value here for the banner).
        /// </summary>
        [HttpGet("notification/server-info")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> GetServerInfo()
        {
            try
            {
                var workerServiceName = _configuration["NotificationWorker:ServiceName"]
                                        ?? "AppNativeNotificationService";
                var pollingMinutes = double.TryParse(
                    _configuration["NotificationWorker:PollingIntervalMinutes"],
                    out var pm) ? pm : 0.1;

                var localNow = DateTime.Now;
                var info = new NotificationServerInfoDto
                {
                    TimeZoneId = TimeZoneInfo.Local.Id,
                    TimeZoneOffsetMinutes = (int)TimeZoneInfo.Local.GetUtcOffset(localNow).TotalMinutes,
                    ServerNowUtc = DateTime.UtcNow,
                    ServerNowLocal = localNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                    WorkerServiceName = workerServiceName,
                    WorkerLastRunUtc = await ToUtcAsync(_repository.GetWorkerLastRunAsync(workerServiceName)),
                    PollingIntervalMinutes = pollingMinutes
                };

                return Ok(new ApiResult { Data = info });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error reading notification server info: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to read server info." });
            }
        }

        /// <summary>
        /// Per-schedule queue health (pending / retry / in-flight counts and
        /// last-activity timestamps) used to surface "schedule on but no enqueues"
        /// situations in the admin UI.
        /// </summary>
        [HttpGet("notification/schedule-stats")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        public async Task<IActionResult> GetScheduleStats([FromQuery] long? tenantId)
        {
            try
            {
                var stats = await _repository.GetSchedulePendingStatsAsync(tenantId);
                return Ok(new ApiResult { Data = stats });
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error reading schedule stats: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to read schedule stats." });
            }
        }

        // SQL Server stores LAST_RUN as DATETIME (server-local). Convert to UTC for
        // the response so the JS client can format in the admin's browser TZ.
        private static async Task<DateTime?> ToUtcAsync(Task<DateTime?> task)
        {
            var local = await task;
            if (!local.HasValue) return null;
            return DateTime.SpecifyKind(local.Value, DateTimeKind.Local).ToUniversalTime();
        }
    }
}
