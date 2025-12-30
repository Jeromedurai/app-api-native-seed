using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Model for Razorpay payment verification request
    /// </summary>
    public class RazorpayPaymentVerificationRequest
    {
        /// <summary>
        /// Razorpay order ID (starts with "order_" from Orders API)
        /// </summary>
        [Required(ErrorMessage = "Order ID is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Order ID is required")]
        public string OrderId { get; set; }

        /// <summary>
        /// Razorpay payment ID (starts with "pay_")
        /// </summary>
        [Required(ErrorMessage = "Payment ID is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Payment ID is required")]
        public string PaymentId { get; set; }

        /// <summary>
        /// Payment signature for verification
        /// </summary>
        [Required(ErrorMessage = "Signature is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Signature is required")]
        public string Signature { get; set; }

        /// <summary>
        /// User ID (optional, for tracking)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional, for multi-tenant support)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Order ID from the system (optional, to link with existing order)
        /// </summary>
        public long? OrderIdInternal { get; set; }

        /// <summary>
        /// Payment amount in paise (optional, may be required for Payment Links)
        /// </summary>
        public long? Amount { get; set; }

        /// <summary>
        /// Session token (optional, for Payment Links)
        /// </summary>
        public string SessionToken { get; set; }
    }

    /// <summary>
    /// Model for Razorpay payment verification response
    /// </summary>
    public class RazorpayPaymentVerificationResponse
    {
        /// <summary>
        /// Verification status (true/false)
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Verification message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Razorpay order ID
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Razorpay payment ID
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// Payment amount in paise
        /// </summary>
        public long? Amount { get; set; }

        /// <summary>
        /// Payment currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Internal database record ID
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Verification timestamp
        /// </summary>
        public DateTime? VerifiedAt { get; set; }
    }
}

