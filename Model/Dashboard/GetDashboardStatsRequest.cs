using System;

namespace Tenant.Query.Model.Dashboard
{
    /// <summary>
    /// Request model for getting dashboard statistics
    /// </summary>
    public class GetDashboardStatsRequest
    {
        /// <summary>
        /// Tenant ID (required)
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Start date (optional, defaults to 30 days ago)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date (optional, defaults to now)
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}

