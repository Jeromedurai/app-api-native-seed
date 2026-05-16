namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Create / update payload for SA_EMAILSCHEDULE rows.
    /// ScheduleId = 0 (or null on the wire) means "create".
    /// </summary>
    public class UpsertEmailScheduleRequest
    {
        public long ScheduleId { get; set; }
        /// <summary>
        /// Tenant scope for the row. NULL = global (super-admin only in practice;
        /// the controller pins this to the caller's tenant for normal admins).
        /// </summary>
        public long? TenantId { get; set; }
        public long TemplateId { get; set; }
        public string ScheduleDescription { get; set; }
        public byte SendBy { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public bool Active { get; set; } = true;
    }
}
