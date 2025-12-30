using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Request model for verifying Razorpay payments
    /// </summary>
    public class VerifyRazorpayPaymentsRequest
    {
        /// <summary>
        /// Razorpay Order ID (starts with "order_" or "plink_")
        /// </summary>
        [Required(ErrorMessage = "Order ID is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Order ID must be between 1 and 255 characters")]
        public string OrderId { get; set; }

        /// <summary>
        /// Razorpay Payment ID (starts with "pay_")
        /// </summary>
        [Required(ErrorMessage = "Payment ID is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Payment ID must be between 1 and 255 characters")]
        public string PaymentId { get; set; }

        /// <summary>
        /// Razorpay signature for verification
        /// </summary>
        [Required(ErrorMessage = "Signature is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Signature must be between 1 and 500 characters")]
        public string Signature { get; set; }

        /// <summary>
        /// Session token from checkout (optional)
        /// </summary>
        [StringLength(500)]
        public string SessionToken { get; set; }

        /// <summary>
        /// Expected amount for validation in paise (optional)
        /// </summary>
        public long? Amount { get; set; }

        /// <summary>
        /// Internal Order ID from your system (optional - for efficiency, avoids database lookup)
        /// </summary>
        public long? InternalOrderId { get; set; }

        /// <summary>
        /// Internal Order Number from your system (optional - alternative to InternalOrderId)
        /// </summary>
        [StringLength(50)]
        public string InternalOrderNumber { get; set; }
    }

    /// <summary>
    /// Response model for verifying Razorpay payments
    /// </summary>
    public class VerifyRazorpayPaymentsResponse
    {
        /// <summary>
        /// Verification status
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Verification message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Amount verified from Razorpay (in paise)
        /// </summary>
        public long? VerifiedAmount { get; set; }

        /// <summary>
        /// Order ID verified from Razorpay
        /// </summary>
        public string VerifiedOrderId { get; set; }

        /// <summary>
        /// Verification timestamp (Unix timestamp in milliseconds)
        /// </summary>
        public long? VerifiedTimestamp { get; set; }

        /// <summary>
        /// Whether order was created in your system
        /// </summary>
        public bool? OrderCreated { get; set; }

        /// <summary>
        /// Your system's order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Your system's order ID
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Payment amount (in paise)
        /// </summary>
        public long? Amount { get; set; }
    }
}
