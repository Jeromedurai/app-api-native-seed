namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Response model for payment status query
    /// </summary>
    public class PaymentStatusResponse
    {
        /// <summary>
        /// Razorpay Order ID
        /// </summary>
        public string RazorpayOrderId { get; set; }

        /// <summary>
        /// Internal Order ID
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Order Number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Payment Status (e.g., "paid", "failed", "cancelled", "pending")
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Order Status (e.g., "confirmed", "pending", "cancelled")
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; }
    }
}
