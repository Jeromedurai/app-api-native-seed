using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Model for creating Razorpay order request
    /// </summary>
    public class RazorpayOrderRequest
    {
        /// <summary>
        /// Amount in paise (₹100 = 10000 paise)
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public long Amount { get; set; }

        /// <summary>
        /// Currency code (must be "INR")
        /// </summary>
        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3 characters")]
        public string Currency { get; set; } = "INR";

        /// <summary>
        /// Unique receipt identifier (auto-generated if not provided)
        /// </summary>
        public string Receipt { get; set; }

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
        public long? OrderId { get; set; }

        /// <summary>
        /// Notes or description (optional)
        /// </summary>
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }

    /// <summary>
    /// Model for Razorpay order response
    /// </summary>
    public class RazorpayOrderResponse
    {
        /// <summary>
        /// Razorpay order ID (starts with "order_")
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Amount in paise
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// Currency code (INR)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Receipt identifier
        /// </summary>
        public string Receipt { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Created timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Internal database record ID
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// Model for creating Razorpay hosted checkout request
    /// </summary>
    public class RazorpayHostedCheckoutRequest
    {
        /// <summary>
        /// Amount in paise (₹100 = 10000 paise)
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public long Amount { get; set; }

        /// <summary>
        /// Currency code (must be "INR")
        /// </summary>
        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3 characters")]
        public string Currency { get; set; } = "INR";

        /// <summary>
        /// Unique receipt identifier (auto-generated if not provided)
        /// </summary>
        public string Receipt { get; set; }

        /// <summary>
        /// Customer name (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Customer name cannot exceed 255 characters")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Customer email (optional)
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Customer email cannot exceed 255 characters")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Customer contact/phone number (optional)
        /// </summary>
        [StringLength(20, ErrorMessage = "Customer contact cannot exceed 20 characters")]
        public string CustomerContact { get; set; }

        /// <summary>
        /// Return URL after successful payment (required)
        /// </summary>
        [Required(ErrorMessage = "Return URL is required")]
        [Url(ErrorMessage = "Return URL must be a valid URL")]
        [StringLength(500, ErrorMessage = "Return URL cannot exceed 500 characters")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Cancel URL when payment is cancelled (required)
        /// </summary>
        [Required(ErrorMessage = "Cancel URL is required")]
        [Url(ErrorMessage = "Cancel URL must be a valid URL")]
        [StringLength(500, ErrorMessage = "Cancel URL cannot exceed 500 characters")]
        public string CancelUrl { get; set; }

        /// <summary>
        /// Cart hash for tamper detection (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Cart hash cannot exceed 255 characters")]
        public string CartHash { get; set; }

        /// <summary>
        /// User ID (optional, for tracking)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Timestamp for expiry check (optional)
        /// </summary>
        public long? Timestamp { get; set; }
    }

    /// <summary>
    /// Model for Razorpay hosted checkout response
    /// </summary>
    public class RazorpayHostedCheckoutResponse
    {
        /// <summary>
        /// Razorpay order ID (starts with "order_")
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Checkout configuration JSON (contains Razorpay Checkout.js options)
        /// Frontend should parse this and use with Razorpay.open()
        /// </summary>
        public string CheckoutUrl { get; set; }

        /// <summary>
        /// Amount in paise
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Session token for additional security (optional)
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// Razorpay Key ID for frontend to initialize Razorpay Checkout
        /// </summary>
        public string RazorpayKeyId { get; set; }
    }
}

