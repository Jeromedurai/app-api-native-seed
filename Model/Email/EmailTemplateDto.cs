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
        public bool Active { get; set; }
    }
}
