namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Read model for SA_EMAILTEMPLATE rows surfaced through the admin API.
    /// </summary>
    public class EmailTemplateDto
    {
        public long TemplateId { get; set; }
        /// <summary>NULL = global (visible to all tenants); non-NULL = scoped to that tenant.</summary>
        public long? TenantId { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string ViewName { get; set; }
        /// <summary>Audience stored procedure for scheduled sends; NULL = event-driven/transactional.</summary>
        public string AudienceSp { get; set; }
        /// <summary>Grouping: Account/Orders/Payments/Cart/Marketing/Reviews/Subscription.</summary>
        public string Category { get; set; }
        /// <summary>Meta-approved WhatsApp template name (separate from the Razor ViewName).</summary>
        public string WhatsAppTemplate { get; set; }
        public bool Active { get; set; }
    }
}
