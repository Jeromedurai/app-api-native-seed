using System;

namespace Tenant.Query.Model.Dashboard
{
    /// <summary>
    /// Sales over time data point
    /// </summary>
    public class SalesOverTimeResponse
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Order count
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Total revenue
        /// </summary>
        public decimal Revenue { get; set; }

        /// <summary>
        /// Paid revenue
        /// </summary>
        public decimal PaidRevenue { get; set; }
    }
}

