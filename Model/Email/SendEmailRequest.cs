using System.Collections.Generic;

namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Request model for sending generic emails
    /// </summary>
    public class SendEmailRequest
    {
        /// <summary>
        /// Recipient email address (required)
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Email subject (required)
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email template name (e.g., "PasswordResetOTP", "WelcomeEmail") (required)
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Data model for the email template (required)
        /// </summary>
        public Dictionary<string, object> TemplateData { get; set; }

        /// <summary>
        /// Optional: Override from email address
        /// </summary>
        public string FromEmail { get; set; }

        /// <summary>
        /// Optional: Override from name
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Optional: CC recipients
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// Optional: BCC recipients
        /// </summary>
        public List<string> Bcc { get; set; }

        /// <summary>
        /// Optional: Reply-to email address
        /// </summary>
        public string ReplyTo { get; set; }
    }
}
