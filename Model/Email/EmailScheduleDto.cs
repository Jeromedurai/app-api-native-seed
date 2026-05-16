namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Read model for SA_EMAILSCHEDULE rows surfaced through the admin API.
    /// SendBy: 0=NotScheduled, 1=Immediate, 2=Daily, 3=Weekly, 4=Monthly.
    /// Day:    Weekly = "0".."6" (Sun-Sat, .NET DayOfWeek);
    ///         Monthly = "1".."31" (CSV allowed, e.g. "1,15").
    /// Time:   ISO time-of-day string (HH:mm or HH:mm:ss). NULL when SendBy = 1.
    /// </summary>
    public class EmailScheduleDto
    {
        public long ScheduleId { get; set; }
        /// <summary>NULL = global schedule; non-NULL = scoped to that tenant.</summary>
        public long? TenantId { get; set; }
        public long TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string ScheduleDescription { get; set; }
        public byte SendBy { get; set; }
        public string SendByLabel { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public bool Active { get; set; }
    }
}
