using System.Collections.Generic;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Razorpay webhook payload — top-level envelope sent by Razorpay to our webhook endpoint.
    /// Razorpay docs: https://razorpay.com/docs/webhooks/
    /// </summary>
    public class RazorpayWebhookPayload
    {
        /// <summary>
        /// Event name, e.g. "payment.captured", "payment.failed", "order.paid"
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Unique webhook event ID — used for idempotency to prevent processing the same event twice.
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Epoch timestamp (seconds) when Razorpay generated the event.
        /// </summary>
        public long? CreatedAt { get; set; }

        /// <summary>
        /// Payload containing entity data. Keys vary by event type.
        /// For payment events: payload.payment.entity contains the payment object.
        /// For order events: payload.order.entity contains the order object.
        /// </summary>
        public Dictionary<string, RazorpayWebhookEntity> Payload { get; set; }
    }

    /// <summary>
    /// Wrapper around a Razorpay entity inside the webhook payload.
    /// </summary>
    public class RazorpayWebhookEntity
    {
        public RazorpayPaymentEntity Entity { get; set; }
    }

    /// <summary>
    /// Razorpay payment entity as returned inside webhook payloads.
    /// Only the fields we actually use are mapped here.
    /// </summary>
    public class RazorpayPaymentEntity
    {
        /// <summary>Razorpay payment ID, e.g. "pay_29QQoUBi66xm2f"</summary>
        public string Id { get; set; }

        /// <summary>Razorpay order ID the payment belongs to, e.g. "order_29QQoUBi66xm2f"</summary>
        public string OrderId { get; set; }

        /// <summary>Amount in paise (smallest currency unit)</summary>
        public long? Amount { get; set; }

        /// <summary>Currency code, e.g. "INR"</summary>
        public string Currency { get; set; }

        /// <summary>Payment status: "captured", "failed", "authorized", "refunded"</summary>
        public string Status { get; set; }

        /// <summary>Error code if payment failed</summary>
        public string ErrorCode { get; set; }

        /// <summary>Human-readable error description if payment failed</summary>
        public string ErrorDescription { get; set; }

        /// <summary>Email provided during payment</summary>
        public string Email { get; set; }

        /// <summary>Contact phone provided during payment</summary>
        public string Contact { get; set; }
    }

    /// <summary>
    /// Result returned by the webhook handler service method.
    /// </summary>
    public class RazorpayWebhookResult
    {
        public bool Processed { get; set; }
        public string Message { get; set; }
        public long? OrderId { get; set; }
        public string OrderNumber { get; set; }
    }
}
