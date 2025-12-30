using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Dashboard;
using Tenant.Query.Service.Dashboard;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Dashboard
{
    /// <summary>
    /// Admin Dashboard Controller
    /// </summary>
    [ApiController]
    [Route("api/1.0/dashboard")]
    [SwaggerTag("Admin Dashboard Management")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        #region Variables
        private readonly DashboardService _dashboardService;
        public ILogger Logger { get; set; }
        #endregion

        public DashboardController(
            DashboardService dashboardService,
            ILoggerFactory loggerFactory)
        {
            _dashboardService = dashboardService;
            this.Logger = loggerFactory.CreateLogger<DashboardController>();
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="startDate">Start date (optional)</param>
        /// <param name="endDate">End date (optional)</param>
        /// <returns>Dashboard statistics</returns>
        [HttpGet("stats")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetDashboardStats(
            [FromQuery] long tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Get tenant ID from claims if not provided
                if (tenantId == 0)
                {
                    var tenantIdClaim = User.FindFirst("TenantId");
                    if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out long claimTenantId))
                    {
                        tenantId = claimTenantId;
                    }
                }

                var stats = await _dashboardService.GetDashboardStats(tenantId, startDate, endDate);

                return Ok(new ApiResult
                {
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting dashboard stats: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Get sales over time
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="startDate">Start date (optional)</param>
        /// <param name="endDate">End date (optional)</param>
        /// <param name="groupBy">Group by: day, week, or month (default: day)</param>
        /// <returns>Sales over time data</returns>
        [HttpGet("sales-over-time")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetSalesOverTime(
            [FromQuery] long tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string groupBy = "day")
        {
            try
            {
                // Get tenant ID from claims if not provided
                if (tenantId == 0)
                {
                    var tenantIdClaim = User.FindFirst("TenantId");
                    if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out long claimTenantId))
                    {
                        tenantId = claimTenantId;
                    }
                }

                var sales = await _dashboardService.GetSalesOverTime(tenantId, startDate, endDate, groupBy);

                return Ok(new ApiResult
                {
                    Data = sales
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting sales over time: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Get top products
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="startDate">Start date (optional)</param>
        /// <param name="endDate">End date (optional)</param>
        /// <param name="topCount">Number of top products to return (default: 10)</param>
        /// <returns>Top products</returns>
        [HttpGet("top-products")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetTopProducts(
            [FromQuery] long tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int topCount = 10)
        {
            try
            {
                // Get tenant ID from claims if not provided
                if (tenantId == 0)
                {
                    var tenantIdClaim = User.FindFirst("TenantId");
                    if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out long claimTenantId))
                    {
                        tenantId = claimTenantId;
                    }
                }

                var products = await _dashboardService.GetTopProducts(tenantId, startDate, endDate, topCount);

                return Ok(new ApiResult
                {
                    Data = products
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting top products: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Get recent orders
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="count">Number of recent orders to return (default: 10)</param>
        /// <returns>Recent orders</returns>
        [HttpGet("recent-orders")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetRecentOrders(
            [FromQuery] long tenantId,
            [FromQuery] int count = 10)
        {
            try
            {
                Logger.LogInformation($"Controller: GetRecentOrders called with tenantId={tenantId}, count={count}");

                // Get tenant ID from claims if not provided
                if (tenantId == 0)
                {
                    var tenantIdClaim = User.FindFirst("TenantId");
                    if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out long claimTenantId))
                    {
                        tenantId = claimTenantId;
                        Logger.LogInformation($"Controller: Using tenantId from claims: {tenantId}");
                    }
                    else
                    {
                        Logger.LogWarning("Controller: tenantId is 0 and no valid claim found");
                        return BadRequest(new ApiResult
                        {
                            Exception = "Tenant ID is required"
                        });
                    }
                }

                // Validate count
                if (count < 1 || count > 100)
                {
                    count = 10;
                    Logger.LogInformation($"Controller: Count adjusted to default: {count}");
                }

                var orders = await _dashboardService.GetRecentOrders(tenantId, count);

                Logger.LogInformation($"Controller: GetRecentOrders returned {orders?.Count ?? 0} orders");

                return Ok(new ApiResult
                {
                    Data = orders ?? new List<RecentOrderResponse>()
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting recent orders: {ex.Message}", ex);
                Logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = $"Error retrieving recent orders: {ex.Message}"
                });
            }
        }
    }
}

