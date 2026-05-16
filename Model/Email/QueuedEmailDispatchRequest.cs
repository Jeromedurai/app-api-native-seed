namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Request payload sent by the AppNativeNotification Windows service for each queued email.
    /// Mirrors AppNativeNotification.Models.EmailSendRequest in the worker repo so the wire format
    /// stays in sync without sharing a project reference.
    /// EMAILSP carries the original payload from SA_EMAILMASTER:
    ///   * "SP_NAME|param1,param2"  → dynamic; API runs the SP to enrich TemplateData
    ///   * "Email~Name~Body"        → static; API renders directly
    /// </summary>
    public class QueuedEmailDispatchRequest
    {
        public long Id { get; set; }
        public long TenantId { get; set; } // The tenant the queue row belongs to.
        public int TemplateId { get; set; }
        public string EmailSP { get; set; } = string.Empty;
        public string CorrelationId { get; set; }
        public int RetryCount { get; set; }
    }
}
