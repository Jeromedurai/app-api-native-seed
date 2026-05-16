namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Response model for email sending operations
    /// </summary>
    public class SendEmailResponse
    {
        /// <summary>
        /// Whether the email was sent successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Recipient email address
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Set by the dispatcher when a queue row is intentionally NOT sent
        /// (e.g. template inactive). Worker treats Skipped + Success=true as
        /// "done, do not retry".
        /// </summary>
        public bool Skipped { get; set; }
    }
}
