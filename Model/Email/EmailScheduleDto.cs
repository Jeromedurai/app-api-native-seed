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
        /// <summary>CSV of UserIds excluded from this campaign; null/empty = all eligible.</summary>
        public string ExcludedUserIds { get; set; }
        /// <summary>Per-campaign coupon code merged into the email as @Model.CouponCode.</summary>
        public string CouponCode { get; set; }
        /// <summary>CSV of channels for this campaign: Email / WhatsApp / Email,WhatsApp. Null = Email.</summary>
        public string Channels { get; set; }
        /// <summary>Admin-authored campaign content (all optional; blank → view's themed fallback).</summary>
        public string Subject { get; set; }
        public string Headline { get; set; }
        public string Message { get; set; }
        public string CtaText { get; set; }
        public string CtaUrl { get; set; }
        public bool Active { get; set; }
    }
}
