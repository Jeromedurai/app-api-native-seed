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
        /// <summary>CSV of UserIds excluded from this campaign; null/empty = send to all eligible.</summary>
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
        public bool Active { get; set; } = true;
    }
}
