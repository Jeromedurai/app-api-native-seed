using System;

namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Per-schedule queue health snapshot returned by SA_GetSchedulePendingStats.
    /// Drives the "Pending" / "Last Activity" columns in the Admin > Notifications page
    /// so admins can spot schedules that are toggled on but receiving no enqueues.
    /// </summary>
    public class SchedulePendingStatsDto
    {
        public long ScheduleId { get; set; }
        public long? TenantId  { get; set; }
        public long TemplateId { get; set; }
        public int  PendingCount  { get; set; }
        public int  RetryCount    { get; set; }
        public int  InFlightCount { get; set; }
        public DateTime? LastEnqueuedAt { get; set; }
        public DateTime? LastSuccessAt  { get; set; }
        public DateTime? LastFailureAt  { get; set; }
    }
}
