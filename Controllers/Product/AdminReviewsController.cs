using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Product;
using Tenant.Query.Service.Product;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Product
{
    /// <summary>
    /// Admin Reviews Management Controller
    /// </summary>
    [ApiController]
    [Route("api/1.0/admin/reviews")]
    [SwaggerTag("Admin Reviews Management")]
    public class AdminReviewsController : ControllerBase
    {
        private readonly ProductReviewService _reviewService;
        public ILogger Logger { get; set; }

        public AdminReviewsController(ProductReviewService reviewService, ILoggerFactory loggerFactory)
        {
            _reviewService = reviewService;
            Logger = loggerFactory.CreateLogger<AdminReviewsController>();
        }

        /// <summary>
        /// Get all reviews (admin view — includes inactive)
        /// </summary>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetAllReviews(
            [FromQuery] long? tenantId = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 50)
        {
            try
            {
                var result = await _reviewService.GetAllReviewsAdmin(tenantId, page, limit);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting all reviews: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Toggle review active/inactive status
        /// </summary>
        [HttpPatch("{reviewId:long}/active")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Review not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> ToggleReviewActive(
            [FromRoute] long reviewId,
            [FromBody] ToggleReviewActiveRequest request)
        {
            try
            {
                var result = await _reviewService.ToggleReviewActive(reviewId, request.Active);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error toggling review active: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }
    }
}
