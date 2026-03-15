using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Contact
{
    /// <summary>
    /// Request payload for the public contact-us form.
    /// </summary>
    public class ContactMessageRequest
    {
        /// <summary>
        /// Optional logged-in user id (if available).
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Optional tenant identifier.
        /// </summary>
        public long? TenantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(32)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; }

        /// <summary>
        /// Optional UI language (e.g. en, hi).
        /// </summary>
        [StringLength(10)]
        public string Language { get; set; }

        /// <summary>
        /// Optional source page or campaign code.
        /// </summary>
        [StringLength(100)]
        public string Source { get; set; }
    }
}

