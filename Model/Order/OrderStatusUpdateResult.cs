using System;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Result model for order status update operations
    /// </summary>
    public class OrderStatusUpdateResult
    {
        /// <summary>
        /// Order ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Razorpay payment ID
        /// </summary>
        public string RazorpayPaymentId { get; set; }

        /// <summary>
        /// Razorpay order ID
        /// </summary>
        public string RazorpayOrderId { get; set; }

        /// <summary>
        /// Updated timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
