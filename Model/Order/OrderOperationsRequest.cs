using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Model for cancel order request
    /// </summary>
    public class CancelOrderRequest
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
        /// Reason for cancellation (optional)
        /// </summary>
        [MaxLength(500, ErrorMessage = "Cancel reason cannot exceed 500 characters")]
        public string CancelReason { get; set; }

        /// <summary>
        /// User ID who cancelled the order (optional, defaults to UserId)
        /// </summary>
        public long? CancelledBy { get; set; }

        /// <summary>
        /// IP address of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for cancel order response
    /// </summary>
    public class CancelOrderResponse
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
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Previous order status
        /// </summary>
        public string PreviousStatus { get; set; }

        /// <summary>
        /// New order status (Cancelled)
        /// </summary>
        public string NewStatus { get; set; }

        /// <summary>
        /// Refund amount (if applicable)
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// Cancellation reason
        /// </summary>
        public string CancelReason { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the order was cancelled
        /// </summary>
        public DateTime CancelledDate { get; set; }

        /// <summary>
        /// Whether a refund was initiated
        /// </summary>
        public bool RefundInitiated { get; set; }

        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Model for update order status request
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        /// <summary>
        /// Order ID (required)
        /// </summary>
        [Required(ErrorMessage = "Order ID is required")]
        public long OrderId { get; set; }

        /// <summary>
        /// User ID (required for customer updates, optional for admin updates)
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// New status (required)
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Status note or comment (optional)
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Status note cannot exceed 1000 characters")]
        public string Note { get; set; }

        /// <summary>
        /// Tracking number (optional, for shipped status)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Tracking number cannot exceed 100 characters")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Shipping carrier (optional)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Carrier cannot exceed 100 characters")]
        public string Carrier { get; set; }

        /// <summary>
        /// Estimated delivery date (optional)
        /// </summary>
        public DateTime? EstimatedDelivery { get; set; }

        /// <summary>
        /// User ID who updated the status (optional, for admin updates)
        /// </summary>
        public long? UpdatedBy { get; set; }

        /// <summary>
        /// IP address of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for bulk update order status request
    /// </summary>
    public class BulkUpdateOrderStatusRequest
    {
        /// <summary>
        /// Order IDs to update (required)
        /// </summary>
        [Required(ErrorMessage = "Order IDs are required")]
        public List<long> OrderIds { get; set; }

        /// <summary>
        /// New status (required)
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Status note or comment (optional)
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Status note cannot exceed 1000 characters")]
        public string Note { get; set; }

        /// <summary>
        /// Admin user ID who is updating the status (required for admin updates)
        /// </summary>
        [Required(ErrorMessage = "Admin User ID is required")]
        public long AdminUserId { get; set; }

        /// <summary>
        /// Tenant ID (required)
        /// </summary>
        [Required(ErrorMessage = "Tenant ID is required")]
        public long TenantId { get; set; }

        /// <summary>
        /// IP address of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent of the client (optional, will be extracted from request if not provided)
        /// </summary>
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// Model for bulk update order status response
    /// </summary>
    public class BulkUpdateOrderStatusResponse
    {
        /// <summary>
        /// Number of orders successfully updated
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// Number of orders that failed to update
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// List of successfully updated orders
        /// </summary>
        public List<UpdateOrderStatusResponse> Orders { get; set; }

        /// <summary>
        /// List of errors for failed updates
        /// </summary>
        public List<string> Errors { get; set; }

        public BulkUpdateOrderStatusResponse()
        {
            Orders = new List<UpdateOrderStatusResponse>();
            Errors = new List<string>();
        }
    }

    /// <summary>
    /// Model for update order status response
    /// </summary>
    public class UpdateOrderStatusResponse
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
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Previous order status
        /// </summary>
        public string PreviousStatus { get; set; }

        /// <summary>
        /// New order status
        /// </summary>
        public string NewStatus { get; set; }

        /// <summary>
        /// Tracking number (if provided)
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Shipping carrier (if provided)
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Estimated delivery date (if provided)
        /// </summary>
        public DateTime? EstimatedDelivery { get; set; }

        /// <summary>
        /// Status note
        /// </summary>
        public string StatusNote { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the status was updated
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// Updated tracking information (if available)
        /// </summary>
        public OrderTrackingInfo TrackingInfo { get; set; }

        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Model for simple operation response
    /// </summary>
    public class SimpleOperationResponse
    {
        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Operation message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional data (optional)
        /// </summary>
        public object Data { get; set; }
    }
}
