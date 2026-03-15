using System;

namespace Tenant.Query.Model.Contact
{
    /// <summary>
    /// Stored contact-us message information.
    /// </summary>
    public class ContactMessageInfo
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? TenantId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Language { get; set; }
        public string Source { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

