using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Request model for marking payment as failed or cancelled
    /// </summary>
    public class MarkPaymentFailedRequest
    {
        /// <summary>
        /// Razorpay Order ID (starts with "order_")
        /// </summary>
        [Required(ErrorMessage = "Razorpay Order ID is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Razorpay Order ID must be between 1 and 255 characters")]
        public string RazorpayOrderId { get; set; }

        /// <summary>
        /// Reason for failure: "cancelled" or "failed"
        /// </summary>
        [Required(ErrorMessage = "Reason is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Reason must be between 1 and 50 characters")]
        public string Reason { get; set; }

        /// <summary>
        /// Optional description of the reason
        /// </summary>
        [StringLength(1000)]
        public string ReasonDescription { get; set; }
    }

    /// <summary>
    /// Response model for marking payment as failed or cancelled
    /// </summary>
    public class MarkPaymentFailedResponse
    {
        /// <summary>
        /// Success status
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Internal Order ID
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Order Number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Updated payment status
        /// </summary>
        public string PaymentStatus { get; set; }
    }
}
