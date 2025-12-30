using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tenant.Query.Model.Dashboard;
using Tenant.Query.Repository.Dashboard;

namespace Tenant.Query.Service.Dashboard
{
    public class DashboardService
    {
        #region Variables
        private readonly DashboardRepository _dashboardRepository;
        public ILogger Logger { get; set; }
        #endregion

        public DashboardService(
            DashboardRepository dashboardRepository,
            ILoggerFactory loggerFactory)
        {
            _dashboardRepository = dashboardRepository;
            this.Logger = loggerFactory.CreateLogger<DashboardService>();
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public async Task<DashboardStatsResponse> GetDashboardStats(long tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting dashboard stats for tenant {tenantId}");

                return await _dashboardRepository.GetDashboardStats(tenantId, startDate, endDate);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting dashboard stats: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get sales over time
        /// </summary>
        public async Task<List<SalesOverTimeResponse>> GetSalesOverTime(long tenantId, DateTime? startDate = null, DateTime? endDate = null, string groupBy = "day")
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting sales over time for tenant {tenantId}");

                // Validate groupBy
                if (groupBy != "day" && groupBy != "week" && groupBy != "month")
                {
                    groupBy = "day";
                }

                return await _dashboardRepository.GetSalesOverTime(tenantId, startDate, endDate, groupBy);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting sales over time: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get top products
        /// </summary>
        public async Task<List<TopProductResponse>> GetTopProducts(long tenantId, DateTime? startDate = null, DateTime? endDate = null, int topCount = 10)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting top products for tenant {tenantId}");

                // Validate topCount
                if (topCount < 1 || topCount > 100)
                {
                    topCount = 10;
                }

                return await _dashboardRepository.GetTopProducts(tenantId, startDate, endDate, topCount);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting top products: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get recent orders
        /// </summary>
        public async Task<List<RecentOrderResponse>> GetRecentOrders(long tenantId, int count = 10)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting recent orders for tenant {tenantId}");

                // Validate count
                if (count < 1 || count > 100)
                {
                    count = 10;
                }

                return await _dashboardRepository.GetRecentOrders(tenantId, count);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting recent orders: {ex.Message}", ex);
                throw;
            }
        }
    }
}

