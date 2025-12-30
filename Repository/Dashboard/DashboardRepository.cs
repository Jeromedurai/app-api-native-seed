using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Constant;
using Tenant.Query.Model.Dashboard;
using Tenant.Query.Uitility;

namespace Tenant.Query.Repository.Dashboard
{
    public class DashboardRepository
    {
        #region Variables
        private readonly Sa.Common.ADO.DataAccess.DataAccess _dataAccess;
        public ILogger Logger { get; set; }
        #endregion

        public DashboardRepository(ProductContext dbContext, ILoggerFactory loggerFactory, Sa.Common.ADO.DataAccess.DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            this.Logger = loggerFactory.CreateLogger<DashboardRepository>();
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public async Task<DashboardStatsResponse> GetDashboardStats(long tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Getting dashboard stats for tenant {tenantId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_DASHBOARD_STATS,
                    tenantId,
                    startDate ?? (object)DBNull.Value,
                    endDate ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return new DashboardStatsResponse();
                }

                var row = result.Tables[0].Rows[0];
                return new DashboardStatsResponse
                {
                    TotalOrders = Convert.ToInt32(row["TotalOrders"]),
                    TotalRevenue = Convert.ToDecimal(row["TotalRevenue"]),
                    TotalCustomers = Convert.ToInt32(row["TotalCustomers"]),
                    TotalProducts = Convert.ToInt32(row["TotalProducts"]),
                    PendingOrders = Convert.ToInt32(row["PendingOrders"]),
                    CompletedOrders = Convert.ToInt32(row["CompletedOrders"]),
                    AverageOrderValue = Convert.ToDecimal(row["AverageOrderValue"])
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting dashboard stats: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Getting sales over time for tenant {tenantId}, groupBy: {groupBy}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_DASHBOARD_SALES_OVER_TIME,
                    tenantId,
                    startDate ?? (object)DBNull.Value,
                    endDate ?? (object)DBNull.Value,
                    groupBy
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    return new List<SalesOverTimeResponse>();
                }

                return result.Tables[0].AsEnumerable()
                    .Select(row => new SalesOverTimeResponse
                    {
                        Date = Convert.ToDateTime(row["Date"]),
                        OrderCount = Convert.ToInt32(row["OrderCount"]),
                        Revenue = Convert.ToDecimal(row["Revenue"]),
                        PaidRevenue = Convert.ToDecimal(row["PaidRevenue"])
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting sales over time: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Getting top products for tenant {tenantId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_DASHBOARD_TOP_PRODUCTS,
                    tenantId,
                    startDate ?? (object)DBNull.Value,
                    endDate ?? (object)DBNull.Value,
                    topCount
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    return new List<TopProductResponse>();
                }

                return result.Tables[0].AsEnumerable()
                    .Select(row => new TopProductResponse
                    {
                        ProductId = Convert.ToInt64(row["ProductId"]),
                        ProductName = row["ProductName"]?.ToString() ?? "",
                        ProductCode = row["ProductCode"]?.ToString() ?? "",
                        TotalQuantitySold = Convert.ToInt32(row["TotalQuantitySold"]),
                        TotalRevenue = Convert.ToDecimal(row["TotalRevenue"]),
                        OrderCount = Convert.ToInt32(row["OrderCount"]),
                        Price = Convert.ToDecimal(row["Price"]),
                        StockQuantity = Convert.ToInt32(row["StockQuantity"])
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting top products: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Getting recent orders for tenant {tenantId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_DASHBOARD_RECENT_ORDERS,
                    tenantId,
                    count
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    return new List<RecentOrderResponse>();
                }

                return result.Tables[0].AsEnumerable()
                    .Select(row => new RecentOrderResponse
                    {
                        OrderId = row["OrderId"] != DBNull.Value ? Convert.ToInt64(row["OrderId"]) : 0,
                        OrderNumber = row["OrderNumber"] != DBNull.Value ? row["OrderNumber"]?.ToString() ?? "" : "",
                        UserId = row["UserId"] != DBNull.Value ? Convert.ToInt64(row["UserId"]) : 0,
                        UserName = row["UserName"] != DBNull.Value ? row["UserName"]?.ToString() ?? "Unknown User" : "Unknown User",
                        UserEmail = row["UserEmail"] != DBNull.Value ? row["UserEmail"]?.ToString() ?? "" : "",
                        TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0,
                        OrderStatus = row["OrderStatus"] != DBNull.Value ? row["OrderStatus"]?.ToString() ?? "Pending" : "Pending",
                        PaymentStatus = row["PaymentStatus"] != DBNull.Value ? row["PaymentStatus"]?.ToString() ?? "Pending" : "Pending",
                        OrderDate = row["OrderDate"] != DBNull.Value ? Convert.ToDateTime(row["OrderDate"]) : DateTime.UtcNow,
                        ItemCount = row["ItemCount"] != DBNull.Value ? Convert.ToInt32(row["ItemCount"]) : 0
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting recent orders: {ex.Message}", ex);
                throw;
            }
        }
    }
}

