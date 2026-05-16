using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Query.Model.Email;
using Tenant.Query.Repository.Email;

namespace Tenant.Query.Service.Email
{
    /// <summary>
    /// Admin-facing CRUD for SA_EMAILTEMPLATE and SA_EMAILSCHEDULE. Validates the
    /// SendBy / Day / Time combination so the worker (which silently filters
    /// ACTIVE = 1) always sees coherent rows.
    ///
    /// Tenant scope: every method takes a tenantId. The controller pins this to
    /// the caller's tenant for normal admins; pass NULL for super-admin / global
    /// scope (the SPs treat NULL as "match global rows only" for writes and
    /// "no scoping filter" for reads, see 02_Create_StoredProcedures.sql).
    /// </summary>
    public class NotificationSettingsService
    {
        private const byte SendByImmediate = 1;
        private const byte SendByDaily     = 2;
        private const byte SendByWeekly    = 3;
        private const byte SendByMonthly   = 4;

        private readonly EmailNotificationRepository _repository;
        private readonly ILogger<NotificationSettingsService> _logger;

        public NotificationSettingsService(
            EmailNotificationRepository repository,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _logger = loggerFactory.CreateLogger<NotificationSettingsService>();
        }

        // ---- Templates ---------------------------------------------------------

        public Task<List<EmailTemplateDto>> GetTemplatesAsync(long? tenantId)
            => _repository.GetAllTemplatesAdminAsync(tenantId);

        public Task<EmailTemplateDto> GetTemplateAsync(long templateId, long? tenantId)
            => _repository.GetTemplateByIdAsync(templateId, tenantId);

        public async Task<EmailTemplateDto> CreateTemplateAsync(UpsertEmailTemplateRequest request, long? tenantId)
        {
            ValidateTemplate(request);
            request.TemplateId = 0;
            var newId = await _repository.UpsertTemplateAsync(request, tenantId);
            return await _repository.GetTemplateByIdAsync(newId, tenantId);
        }

        public async Task<EmailTemplateDto> UpdateTemplateAsync(long templateId, UpsertEmailTemplateRequest request, long? tenantId)
        {
            if (templateId <= 0) throw new ArgumentException("TemplateId is required.");
            ValidateTemplate(request);
            request.TemplateId = templateId;
            await _repository.UpsertTemplateAsync(request, tenantId);
            return await _repository.GetTemplateByIdAsync(templateId, tenantId);
        }

        public Task<int> DeleteTemplateAsync(long templateId, long? tenantId)
        {
            if (templateId <= 0) throw new ArgumentException("TemplateId is required.");
            return _repository.DeleteTemplateAsync(templateId, tenantId);
        }

        public Task<int> SetTemplateActiveAsync(long templateId, bool active, long? tenantId)
        {
            if (templateId <= 0) throw new ArgumentException("TemplateId is required.");
            return _repository.SetTemplateActiveAsync(templateId, active, tenantId);
        }

        // ---- Schedules ---------------------------------------------------------

        public Task<List<EmailScheduleDto>> GetSchedulesAsync(long? tenantId)
            => _repository.GetAllSchedulesAdminAsync(tenantId);

        public async Task<EmailScheduleDto> CreateScheduleAsync(UpsertEmailScheduleRequest request, long? tenantId)
        {
            ValidateSchedule(request);
            request.ScheduleId = 0;
            var newId = await _repository.UpsertScheduleAsync(request, tenantId);
            var all = await _repository.GetAllSchedulesAdminAsync(tenantId);
            return all.FirstOrDefault(s => s.ScheduleId == newId);
        }

        public async Task<EmailScheduleDto> UpdateScheduleAsync(long scheduleId, UpsertEmailScheduleRequest request, long? tenantId)
        {
            if (scheduleId <= 0) throw new ArgumentException("ScheduleId is required.");
            ValidateSchedule(request);
            request.ScheduleId = scheduleId;
            await _repository.UpsertScheduleAsync(request, tenantId);
            var all = await _repository.GetAllSchedulesAdminAsync(tenantId);
            return all.FirstOrDefault(s => s.ScheduleId == scheduleId);
        }

        public Task<int> DeleteScheduleAsync(long scheduleId, long? tenantId)
        {
            if (scheduleId <= 0) throw new ArgumentException("ScheduleId is required.");
            return _repository.DeleteScheduleAsync(scheduleId, tenantId);
        }

        public Task<int> SetScheduleActiveAsync(long scheduleId, bool active, long? tenantId)
        {
            if (scheduleId <= 0) throw new ArgumentException("ScheduleId is required.");
            return _repository.SetScheduleActiveAsync(scheduleId, active, tenantId);
        }

        // ---- Validation --------------------------------------------------------

        private static void ValidateTemplate(UpsertEmailTemplateRequest request)
        {
            if (request == null) throw new ArgumentException("Template payload is required.");
            if (string.IsNullOrWhiteSpace(request.TemplateName))
                throw new ArgumentException("TemplateName is required.");
            if (request.TemplateName.Length > 100)
                throw new ArgumentException("TemplateName must be 100 characters or fewer.");
            if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 255)
                throw new ArgumentException("Description must be 255 characters or fewer.");
            if (!string.IsNullOrEmpty(request.ViewName) && request.ViewName.Length > 100)
                throw new ArgumentException("ViewName must be 100 characters or fewer.");
        }

        private static void ValidateSchedule(UpsertEmailScheduleRequest request)
        {
            if (request == null) throw new ArgumentException("Schedule payload is required.");
            if (request.TemplateId <= 0)
                throw new ArgumentException("TemplateId is required.");
            if (string.IsNullOrWhiteSpace(request.ScheduleDescription))
                throw new ArgumentException("ScheduleDescription is required.");
            if (request.ScheduleDescription.Length > 100)
                throw new ArgumentException("ScheduleDescription must be 100 characters or fewer.");

            switch (request.SendBy)
            {
                case 0: // NotScheduled — no further checks
                    return;
                case SendByImmediate:
                    return;
                case SendByDaily:
                    if (string.IsNullOrWhiteSpace(request.Time))
                        throw new ArgumentException("Time is required when SendBy is Daily.");
                    return;
                case SendByWeekly:
                    if (string.IsNullOrWhiteSpace(request.Time))
                        throw new ArgumentException("Time is required when SendBy is Weekly.");
                    if (string.IsNullOrWhiteSpace(request.Day))
                        throw new ArgumentException("Day is required (CSV of 0-6) when SendBy is Weekly.");
                    foreach (var token in SplitTokens(request.Day))
                    {
                        if (!int.TryParse(token, out var d) || d < 0 || d > 6)
                            throw new ArgumentException($"Weekly Day token '{token}' must be 0-6 (Sun..Sat).");
                    }
                    return;
                case SendByMonthly:
                    if (string.IsNullOrWhiteSpace(request.Time))
                        throw new ArgumentException("Time is required when SendBy is Monthly.");
                    if (string.IsNullOrWhiteSpace(request.Day))
                        throw new ArgumentException("Day is required (CSV of 1-31) when SendBy is Monthly.");
                    foreach (var token in SplitTokens(request.Day))
                    {
                        if (!int.TryParse(token, out var d) || d < 1 || d > 31)
                            throw new ArgumentException($"Monthly Day token '{token}' must be 1-31.");
                    }
                    return;
                default:
                    throw new ArgumentException($"SendBy {request.SendBy} is invalid (allowed: 0,1,2,3,4).");
            }
        }

        private static IEnumerable<string> SplitTokens(string day) =>
            day.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).Where(t => t.Length > 0);
    }
}
