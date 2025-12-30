using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Admin
{
    /// <summary>
    /// Model for admin get all users request
    /// </summary>
    public class GetAllUsersRequest
    {
        /// <summary>
        /// Admin User ID (required)
        /// </summary>
        [Required(ErrorMessage = "Admin User ID is required")]
        public long AdminUserId { get; set; }

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
        /// Items per page (default: 10, max: 100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Search by name or email (optional)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string Search { get; set; }

        /// <summary>
        /// Filter by user role (optional)
        /// </summary>
        [MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        public string Role { get; set; }

        /// <summary>
        /// Filter by account status (optional: active, locked, inactive)
        /// </summary>
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Model for admin get all users response
    /// </summary>
    public class GetAllUsersResponse
    {
        /// <summary>
        /// List of users
        /// </summary>
        public List<AdminUserInfo> Users { get; set; } = new List<AdminUserInfo>();

        /// <summary>
        /// Pagination information
        /// </summary>
        public Model.Order.PaginationInfo Pagination { get; set; }
    }

    /// <summary>
    /// Model for admin user information
    /// </summary>
    public class AdminUserInfo
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Role ID
        /// </summary>
        public long? RoleId { get; set; }

        /// <summary>
        /// Avatar/Profile picture URL
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Active status
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Email verification status
        /// </summary>
        public bool EmailVerified { get; set; }

        /// <summary>
        /// Phone verification status
        /// </summary>
        public bool PhoneVerified { get; set; }

        /// <summary>
        /// Account status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Account creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Number of orders placed
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Total amount spent
        /// </summary>
        public decimal TotalSpent { get; set; }

        /// <summary>
        /// Last order date
        /// </summary>
        public DateTime? LastOrderDate { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State/Province
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Postal/ZIP code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Job title
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// User permissions
        /// </summary>
        public List<UserPermissionInfo> Permissions { get; set; } = new List<UserPermissionInfo>();
    }

    /// <summary>
    /// Model for user permission information
    /// </summary>
    public class UserPermissionInfo
    {
        /// <summary>
        /// Permission name
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// Permission description
        /// </summary>
        public string PermissionDescription { get; set; }

        /// <summary>
        /// Permission source (Role-based or Custom)
        /// </summary>
        public string PermissionSource { get; set; }
    }

    /// <summary>
    /// Model for admin update user role request
    /// </summary>
    public class UpdateUserRoleRequest
    {
        /// <summary>
        /// Admin User ID (required)
        /// </summary>
        [Required(ErrorMessage = "Admin User ID is required")]
        public long AdminUserId { get; set; }

        /// <summary>
        /// Target User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// New role name (required)
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        [MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        public string Role { get; set; }

        /// <summary>
        /// Custom permissions (optional)
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();

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
    /// Model for admin update user status request
    /// </summary>
    public class UpdateUserStatusRequest
    {
        /// <summary>
        /// Admin User ID (required)
        /// </summary>
        [Required(ErrorMessage = "Admin User ID is required")]
        public long AdminUserId { get; set; }

        /// <summary>
        /// Target User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// New status: "active" or "inactive" (required)
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(active|inactive)$", ErrorMessage = "Status must be either 'active' or 'inactive'")]
        public string Status { get; set; }

        /// <summary>
        /// Reason for status change (optional)
        /// </summary>
        [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; }

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
    /// Model for admin update user role response
    /// </summary>
    public class UpdateUserRoleResponse
    {
        /// <summary>
        /// Target user ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Target user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Previous role
        /// </summary>
        public string PreviousRole { get; set; }

        /// <summary>
        /// New role
        /// </summary>
        public string NewRole { get; set; }

        /// <summary>
        /// Admin user who made the update
        /// </summary>
        public long UpdatedBy { get; set; }

        /// <summary>
        /// Update timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Assigned permissions after role update
        /// </summary>
        public List<UserPermissionInfo> AssignedPermissions { get; set; } = new List<UserPermissionInfo>();

        /// <summary>
        /// Success indicator
        /// </summary>
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Model for admin get all orders request
    /// </summary>
    public class GetAllOrdersRequest
    {
        /// <summary>
        /// Admin User ID (required)
        /// </summary>
        [Required(ErrorMessage = "Admin User ID is required")]
        public long AdminUserId { get; set; }

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
        /// Items per page (default: 10, max: 100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Filter by order status (optional)
        /// </summary>
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Search by order number or customer (optional)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string Search { get; set; }

        /// <summary>
        /// Start date for date range filter (optional)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date for date range filter (optional)
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Model for admin get all orders response
    /// </summary>
    public class GetAllOrdersResponse
    {
        /// <summary>
        /// List of orders
        /// </summary>
        public List<AdminOrderInfo> Orders { get; set; } = new List<AdminOrderInfo>();

        /// <summary>
        /// Pagination information
        /// </summary>
        public Model.Order.PaginationInfo Pagination { get; set; }

        /// <summary>
        /// Order statistics summary
        /// </summary>
        public OrderStatsSummary Statistics { get; set; }
    }

    /// <summary>
    /// Model for admin order information
    /// </summary>
    public class AdminOrderInfo
    {
        /// <summary>
        /// Order ID
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Order number
        /// </summary>
        public string OrderNumber { get; set; }

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
        /// Order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Subtotal
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
        /// Number of items
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Total quantity
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Order creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Shipped date
        /// </summary>
        public DateTime? ShippedAt { get; set; }

        /// <summary>
        /// Delivered date
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// Estimated delivery date
        /// </summary>
        public DateTime? EstimatedDelivery { get; set; }

        /// <summary>
        /// Shipping address
        /// </summary>
        public Tenant.Query.Model.Order.AddressRequest ShippingAddress { get; set; }

        /// <summary>
        /// Billing address
        /// </summary>
        public Tenant.Query.Model.Order.AddressRequest BillingAddress { get; set; }

        /// <summary>
        /// Payment method
        /// </summary>
        public Tenant.Query.Model.Order.PaymentMethodRequest PaymentMethod { get; set; }

        /// <summary>
        /// Shipping method
        /// </summary>
        public Tenant.Query.Model.Order.ShippingMethodRequest ShippingMethod { get; set; }

        /// <summary>
        /// Order notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Customer ID
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        /// Customer tenant ID
        /// </summary>
        public long? CustomerTenantId { get; set; }

        /// <summary>
        /// Order items
        /// </summary>
        public List<AdminOrderItemInfo> Items { get; set; } = new List<AdminOrderItemInfo>();
    }

    /// <summary>
    /// Model for admin order item information
    /// </summary>
    public class AdminOrderItemInfo
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
        /// Product image
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity ordered
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Total price (price * quantity)
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
        /// Product offer
        /// </summary>
        public string Offer { get; set; }
    }

    /// <summary>
    /// Model for order statistics summary
    /// </summary>
    public class OrderStatsSummary
    {
        /// <summary>
        /// Total number of orders
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Total revenue
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Average order value
        /// </summary>
        public decimal AverageOrderValue { get; set; }

        /// <summary>
        /// Number of unique customers
        /// </summary>
        public int UniqueCustomers { get; set; }

        /// <summary>
        /// Number of pending orders
        /// </summary>
        public int PendingOrders { get; set; }

        /// <summary>
        /// Number of processing orders
        /// </summary>
        public int ProcessingOrders { get; set; }

        /// <summary>
        /// Number of shipped orders
        /// </summary>
        public int ShippedOrders { get; set; }

        /// <summary>
        /// Number of delivered orders
        /// </summary>
        public int DeliveredOrders { get; set; }

        /// <summary>
        /// Number of cancelled orders
        /// </summary>
        public int CancelledOrders { get; set; }
    }
}
