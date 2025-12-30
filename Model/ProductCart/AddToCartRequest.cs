using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.ProductCart
{
    /// <summary>
    /// Model for adding item to cart request
    /// </summary>
    public class AddToCartRequest
    {
        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Product ID (required)
        /// </summary>
        [Required(ErrorMessage = "Product ID is required")]
        public long ProductId { get; set; }

        /// <summary>
        /// Quantity to add/subtract (required, positive = add, negative = subtract, cannot be zero)
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        /// <summary>
        /// Custom validation to ensure quantity is not zero
        /// </summary>
        public bool IsQuantityValid()
        {
            return Quantity != 0;
        }

        /// <summary>
        /// Tenant ID (optional for filtering)
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// Session ID (optional for guest cart support)
        /// </summary>
        public string SessionId { get; set; }

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
    /// Model for add to cart response
    /// </summary>
    public class AddToCartResponse
    {
        /// <summary>
        /// Cart ID
        /// </summary>
        public long CartId { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Quantity in cart
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total item price (quantity * price)
        /// </summary>
        public decimal ItemTotal { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date when added/updated
        /// </summary>
        public System.DateTime UpdatedDate { get; set; }

        /// <summary>
        /// Operation type (INCREASE, DECREASE, REMOVED)
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// Cart summary
        /// </summary>
        public CartSummaryInfo Summary { get; set; } = new CartSummaryInfo();
    }

    /// <summary>
    /// Model for cart summary information
    /// </summary>
    public class CartSummaryInfo
    {
        /// <summary>
        /// Total unique items in cart
        /// </summary>
        public int TotalUniqueItems { get; set; }

        /// <summary>
        /// Total quantity of all items
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Total amount of all items
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
