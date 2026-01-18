using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Model for updating order status with payment information
    /// </summary>
    public class UpdateOrderStatusWithPaymentRequest
    {
        /// <summary>
        /// Internal order ID (database ID) - required if orderNumber is not provided
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Order number (e.g., "ORD-2024-001234") - used if orderId is not available
        /// </summary>
        [MaxLength(50, ErrorMessage = "Order number cannot exceed 50 characters")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Order status (e.g., "pending", "confirmed", "processing", "shipped", "delivered", "cancelled")
        /// </summary>
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Payment status (e.g., "pending", "paid", "failed", "refunded", "partial")
        /// </summary>
        [MaxLength(50, ErrorMessage = "Payment status cannot exceed 50 characters")]
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Razorpay payment ID (starts with "pay_") - for Razorpay payments
        /// </summary>
        [MaxLength(255, ErrorMessage = "Razorpay payment ID cannot exceed 255 characters")]
        public string RazorpayPaymentId { get; set; }

        /// <summary>
        /// Razorpay order ID (starts with "order_") - for Razorpay payments
        /// </summary>
        [MaxLength(255, ErrorMessage = "Razorpay order ID cannot exceed 255 characters")]
        public string RazorpayOrderId { get; set; }

        /// <summary>
        /// Razorpay payment signature - for Razorpay payments
        /// </summary>
        [MaxLength(500, ErrorMessage = "Razorpay signature cannot exceed 500 characters")]
        public string RazorpaySignature { get; set; }

        /// <summary>
        /// Additional notes or comments about the status update
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string Notes { get; set; }

        /// <summary>
        /// User ID who is updating the order (for audit trail)
        /// </summary>
        public long? UpdatedBy { get; set; }
    }

    /// <summary>
    /// Model for update order status with payment response
    /// </summary>
    public class UpdateOrderStatusWithPaymentResponse
    {
        /// <summary>
        /// Updated order ID (nullable - can be null if order doesn't exist yet)
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Updated order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Updated payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Razorpay payment ID (if applicable)
        /// </summary>
        public string RazorpayPaymentId { get; set; }

        /// <summary>
        /// Razorpay order ID (if applicable)
        /// </summary>
        public string RazorpayOrderId { get; set; }

        /// <summary>
        /// Timestamp of update (ISO 8601)
        /// </summary>
        public System.DateTime UpdatedAt { get; set; }

        /// <summary>
        /// User ID who updated the order
        /// </summary>
        public long? UpdatedBy { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }
    }
}

