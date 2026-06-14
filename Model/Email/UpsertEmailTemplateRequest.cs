namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Create / update payload for SA_EMAILTEMPLATE rows.
    /// TemplateId = 0 (or null on the wire) means "create".
    /// </summary>
    public class UpsertEmailTemplateRequest
    {
        public long TemplateId { get; set; }
        /// <summary>
        /// Tenant scope for the row. NULL = global (super-admin only in practice;
        /// the controller pins this to the caller's tenant for normal admins).
        /// </summary>
        public long? TenantId { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string ViewName { get; set; }
        /// <summary>Audience stored procedure for scheduled sends; NULL/blank = event-driven/transactional.</summary>
        public string AudienceSp { get; set; }
        /// <summary>Grouping: Account/Orders/Payments/Cart/Marketing/Reviews/Subscription.</summary>
        public string Category { get; set; }
        /// <summary>Meta-approved WhatsApp template name (separate from the Razor ViewName).</summary>
        public string WhatsAppTemplate { get; set; }
        public bool Active { get; set; } = true;
    }
}
