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
        /// Resolve the tenant id from the query parameter, falling back to the
        /// authenticated user's "TenantId" claim. Returns false when neither yields
        /// a positive tenant id.
        /// </summary>
        private bool TryResolveTenantId(ref long tenantId)
        {
            if (tenantId > 0)
            {
                return true;
            }

            var tenantIdClaim = User.FindFirst("TenantId");
            if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out long claimTenantId) && claimTenantId > 0)
            {
                tenantId = claimTenantId;
                return true;
            }

            return false;
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
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetDashboardStats(
            [FromQuery] long tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (!TryResolveTenantId(ref tenantId))
            {
                return BadRequest(new ApiResult { Exception = "Tenant ID is required" });
            }

            try
            {
                var stats = await _dashboardService.GetDashboardStats(tenantId, startDate, endDate);

                return Ok(new ApiResult
                {
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting dashboard stats for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = "An error occurred while retrieving dashboard statistics."
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
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetSalesOverTime(
            [FromQuery] long tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string groupBy = "day")
        {
            if (!TryResolveTenantId(ref tenantId))
            {
                return BadRequest(new ApiResult { Exception = "Tenant ID is required" });
            }

            try
            {
                var sales = await _dashboardService.GetSalesOverTime(tenantId, startDate, endDate, groupBy);

                return Ok(new ApiResult
                {
                    Data = sales
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting sales over time for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = "An error occurred while retrieving sales over time."
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
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetTopProducts(
            [FromQuery] long tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int topCount = 10)
        {
            if (!TryResolveTenantId(ref tenantId))
            {
                return BadRequest(new ApiResult { Exception = "Tenant ID is required" });
            }

            try
            {
                var products = await _dashboardService.GetTopProducts(tenantId, startDate, endDate, topCount);

                return Ok(new ApiResult
                {
                    Data = products
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting top products for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = "An error occurred while retrieving top products."
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
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetRecentOrders(
            [FromQuery] long tenantId,
            [FromQuery] int count = 10)
        {
            if (!TryResolveTenantId(ref tenantId))
            {
                Logger.LogWarning("Controller: tenantId is 0 and no valid claim found");
                return BadRequest(new ApiResult { Exception = "Tenant ID is required" });
            }

            try
            {
                Logger.LogDebug("Controller: GetRecentOrders called with tenantId={TenantId}, count={Count}", tenantId, count);

                // Validate count
                if (count < 1 || count > 100)
                {
                    count = 10;
                }

                var orders = await _dashboardService.GetRecentOrders(tenantId, count);

                Logger.LogDebug("Controller: GetRecentOrders returned {Count} orders", orders?.Count ?? 0);

                return Ok(new ApiResult
                {
                    Data = orders ?? new List<RecentOrderResponse>()
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting recent orders for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = "An error occurred while retrieving recent orders."
                });
            }
        }
    }
}
