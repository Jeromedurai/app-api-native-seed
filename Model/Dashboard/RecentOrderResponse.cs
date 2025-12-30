using System;

namespace Tenant.Query.Model.Dashboard
{
    /// <summary>
    /// Recent order response
    /// </summary>
    public class RecentOrderResponse
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
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Order date
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Item count
        /// </summary>
        public int ItemCount { get; set; }
    }
}

