using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Dashboard
{
    /// <summary>
    /// Dashboard statistics response
    /// </summary>
    public class DashboardStatsResponse
    {
        /// <summary>
        /// Total orders
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Total revenue
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Total customers
        /// </summary>
        public int TotalCustomers { get; set; }

        /// <summary>
        /// Total products
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// Pending orders
        /// </summary>
        public int PendingOrders { get; set; }

        /// <summary>
        /// Completed orders
        /// </summary>
        public int CompletedOrders { get; set; }

        /// <summary>
        /// Average order value
        /// </summary>
        public decimal AverageOrderValue { get; set; }
    }
}

