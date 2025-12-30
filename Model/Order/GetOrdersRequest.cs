using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Model for getting orders request
    /// </summary>
    public class GetOrdersRequest
    {
        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Page number (default: 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 10)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Filter by order status (optional)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Search by order number (optional)
        /// </summary>
        public string Search { get; set; }
    }

    /// <summary>
    /// Model for get orders response
    /// </summary>
    public class GetOrdersResponse
    {
        /// <summary>
        /// List of orders
        /// </summary>
        public List<OrderListItem> Orders { get; set; } = new List<OrderListItem>();

        /// <summary>
        /// Pagination information
        /// </summary>
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    /// <summary>
    /// Model for order list item
    /// </summary>
    public class OrderListItem
    {
        /// <summary>
        /// Order ID
        /// </summary>
        public long Id { get; set; }

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
        /// Total order amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Subtotal amount
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Shipping amount
        /// </summary>
        public decimal ShippingAmount { get; set; }

        /// <summary>
        /// Tax amount
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Discount amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Coupon ID (if coupon was applied)
        /// </summary>
        public long? CouponId { get; set; }

        /// <summary>
        /// Coupon code (if coupon was applied)
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Coupon discount amount (if coupon was applied)
        /// </summary>
        public decimal? CouponDiscountAmount { get; set; }

        /// <summary>
        /// Order notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Order creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Order last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Total number of unique items
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total quantity of all items
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// List of order items
        /// </summary>
        public List<OrderListItemInfo> Items { get; set; } = new List<OrderListItemInfo>();
    }

    /// <summary>
    /// Model for order item information in list
    /// </summary>
    public class OrderListItemInfo
    {
        /// <summary>
        /// Order item ID
        /// </summary>
        public long OrderItemId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product image URL
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity ordered
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Total price for this item
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Product rating
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Product offer/discount information
        /// </summary>
        public string Offer { get; set; }
    }

    /// <summary>
    /// Model for pagination information
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Total number of items
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPrevious { get; set; }
    }

    /// <summary>
    /// Model for get order by ID request
    /// </summary>
    public class GetOrderByIdRequest
    {
        /// <summary>
        /// Order ID (required)
        /// </summary>
        [Required(ErrorMessage = "Order ID is required")]
        public long OrderId { get; set; }

        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Include order items in response (default: true)
        /// </summary>
        public bool IncludeItems { get; set; } = true;

        /// <summary>
        /// Include shipping/billing addresses in response (default: true)
        /// </summary>
        public bool IncludeAddress { get; set; } = true;

        /// <summary>
        /// Include payment details in response (default: true)
        /// </summary>
        public bool IncludePayment { get; set; } = true;
    }

    /// <summary>
    /// Model for get order by ID response
    /// </summary>
    public class GetOrderByIdResponse
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
        public string OrderStatus { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Total order amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Subtotal amount
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Shipping amount
        /// </summary>
        public decimal ShippingAmount { get; set; }

        /// <summary>
        /// Tax amount
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Discount amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Order notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Shipping address (JSON)
        /// </summary>
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Billing address (JSON)
        /// </summary>
        public string BillingAddress { get; set; }

        /// <summary>
        /// Payment method (JSON)
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Shipping method (JSON)
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// Order creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Order last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Customer name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Customer email
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Customer phone
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// Total number of unique items
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total quantity of all items
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// List of order items
        /// </summary>
        public List<OrderDetailItemInfo> Items { get; set; } = new List<OrderDetailItemInfo>();

        /// <summary>
        /// Order status history
        /// </summary>
        public List<OrderStatusHistoryInfo> StatusHistory { get; set; } = new List<OrderStatusHistoryInfo>();

        /// <summary>
        /// Order tracking information
        /// </summary>
        public List<OrderTrackingInfo> TrackingInfo { get; set; } = new List<OrderTrackingInfo>();

        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Order type
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Coupon ID (if coupon was applied)
        /// </summary>
        public long? CouponId { get; set; }

        /// <summary>
        /// Coupon code (if coupon was applied)
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Coupon discount amount (if coupon was applied)
        /// </summary>
        public decimal? CouponDiscountAmount { get; set; }

        /// <summary>
        /// Applied discount information (JSON)
        /// </summary>
        public string AppliedDiscount { get; set; }
    }

    /// <summary>
    /// Model for order detail response matching API specification
    /// </summary>
    public class OrderDetailResponse
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
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Order type
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Order totals
        /// </summary>
        public OrderTotals Totals { get; set; }

        /// <summary>
        /// Order items
        /// </summary>
        public List<OrderItemDetail> Items { get; set; } = new List<OrderItemDetail>();

        /// <summary>
        /// Shipping address
        /// </summary>
        public OrderAddress ShippingAddress { get; set; }

        /// <summary>
        /// Billing address
        /// </summary>
        public OrderAddress BillingAddress { get; set; }

        /// <summary>
        /// Shipping method
        /// </summary>
        public OrderShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// Payment method
        /// </summary>
        public OrderPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Applied discount
        /// </summary>
        public object AppliedDiscount { get; set; }

        /// <summary>
        /// Coupon ID (if coupon was applied)
        /// </summary>
        public long? CouponId { get; set; }

        /// <summary>
        /// Coupon code (if coupon was applied)
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Coupon discount amount (if coupon was applied)
        /// </summary>
        public decimal? CouponDiscountAmount { get; set; }

        /// <summary>
        /// Order notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Created timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Updated timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Model for order totals
    /// </summary>
    public class OrderTotals
    {
        /// <summary>
        /// Subtotal amount
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Shipping amount
        /// </summary>
        public decimal Shipping { get; set; }

        /// <summary>
        /// Tax amount
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// Discount amount
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Currency code
        /// </summary>
        public string Currency { get; set; }
    }

    /// <summary>
    /// Model for order item detail
    /// </summary>
    public class OrderItemDetail
    {
        /// <summary>
        /// Order item ID
        /// </summary>
        public long OrderItemId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Product image URL
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Total price for this item
        /// </summary>
        public decimal Total { get; set; }
    }

    /// <summary>
    /// Model for order address
    /// </summary>
    public class OrderAddress
    {
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Address line 1
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Address line 2
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// ZIP code
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }
    }

    /// <summary>
    /// Model for order shipping method
    /// </summary>
    public class OrderShippingMethod
    {
        /// <summary>
        /// Shipping method ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Shipping method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Estimated delivery days
        /// </summary>
        public string EstimatedDays { get; set; }

        /// <summary>
        /// Carrier name
        /// </summary>
        public string Carrier { get; set; }
    }

    /// <summary>
    /// Model for order payment method
    /// </summary>
    public class OrderPaymentMethod
    {
        /// <summary>
        /// Payment method ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Payment method type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Payment method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Payment ID (if payment completed)
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// Razorpay order ID
        /// </summary>
        public string RazorpayOrderId { get; set; }

        /// <summary>
        /// Razorpay payment ID
        /// </summary>
        public string RazorpayPaymentId { get; set; }
    }

    /// <summary>
    /// Model for detailed order item information
    /// </summary>
    public class OrderDetailItemInfo
    {
        /// <summary>
        /// Order item ID
        /// </summary>
        public long OrderItemId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product image URL
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity ordered
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Total price for this item
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Item creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Product description
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Product rating
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Product offer/discount information
        /// </summary>
        public string Offer { get; set; }

        /// <summary>
        /// Whether product is in stock
        /// </summary>
        public bool InStock { get; set; }

        /// <summary>
        /// Whether product is a best seller
        /// </summary>
        public bool BestSeller { get; set; }
    }

    /// <summary>
    /// Model for order status history information
    /// </summary>
    public class OrderStatusHistoryInfo
    {
        /// <summary>
        /// Status history ID
        /// </summary>
        public long StatusHistoryId { get; set; }

        /// <summary>
        /// Previous status
        /// </summary>
        public string PreviousStatus { get; set; }

        /// <summary>
        /// New status
        /// </summary>
        public string NewStatus { get; set; }

        /// <summary>
        /// Status change note
        /// </summary>
        public string StatusNote { get; set; }

        /// <summary>
        /// User ID who changed the status
        /// </summary>
        public long ChangedBy { get; set; }

        /// <summary>
        /// Name of user who changed the status
        /// </summary>
        public string ChangedByName { get; set; }

        /// <summary>
        /// Date and time of status change
        /// </summary>
        public DateTime ChangedAt { get; set; }
    }

    /// <summary>
    /// Model for order tracking information
    /// </summary>
    public class OrderTrackingInfo
    {
        /// <summary>
        /// Tracking ID
        /// </summary>
        public long TrackingId { get; set; }

        /// <summary>
        /// Tracking number
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Shipping carrier
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Tracking status
        /// </summary>
        public string TrackingStatus { get; set; }

        /// <summary>
        /// Estimated delivery date
        /// </summary>
        public DateTime? EstimatedDelivery { get; set; }

        /// <summary>
        /// Actual delivery date
        /// </summary>
        public DateTime? ActualDelivery { get; set; }

        /// <summary>
        /// Tracking URL
        /// </summary>
        public string TrackingUrl { get; set; }

        /// <summary>
        /// Tracking creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Tracking last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
