using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    /// Product Reviews Controller
    /// </summary>
    [ApiController]
    [Route("api/1.0/productreview/{productId}/reviews")]
    [SwaggerTag("Product Reviews Management")]
    public class ProductReviewsController : ControllerBase
    {
        #region Variables
        private const string GenericErrorMessage = "An error occurred while processing your request.";
        private readonly ProductReviewService _reviewService;
        private readonly ILogger<ProductReviewsController> _logger;
        #endregion

        public ProductReviewsController(
            ProductReviewService reviewService,
            ILoggerFactory loggerFactory)
        {
            _reviewService = reviewService;
            _logger = loggerFactory.CreateLogger<ProductReviewsController>();
        }

        /// <summary>
        /// Resolve the authenticated user id from JWT claims only. Returns false when absent.
        /// </summary>
        private bool TryGetUserId(out long userId)
        {
            // Claim names come from the compiled DLL (CLAIM_USER_ID); read resiliently
            // using the same names already proven in ProductsController/BannersController.
            var claim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?? User?.FindFirst("user_id")
                ?? User?.FindFirst("userId")
                ?? User?.FindFirst("UserId")
                ?? User?.FindFirst("sub");
            return long.TryParse(claim?.Value, out userId) && userId > 0;
        }

        /// <summary>
        /// Resolve the authenticated tenant id from JWT claims only. Returns false when absent.
        /// </summary>
        private bool TryGetTenantId(out long tenantId)
        {
            // CLAIM_TENANT_ID from the compiled DLL; ProductsController proves "tenant_id"/"tenantId".
            var claim = User?.FindFirst("tenant_id")
                ?? User?.FindFirst("tenantId")
                ?? User?.FindFirst("TenantId");
            return long.TryParse(claim?.Value, out tenantId) && tenantId > 0;
        }

        /// <summary>
        /// Get reviews for a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <returns>List of reviews</returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetReviews(
            [FromRoute] long productId,
            [FromQuery] long? tenantId = null)
        {
            try
            {
                var reviews = await _reviewService.GetReviews(productId, tenantId);

                return Ok(new ApiResult
                {
                    Data = reviews
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product {ProductId}", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Get review statistics for a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <returns>Review statistics</returns>
        [HttpGet("stats")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> GetReviewStats(
            [FromRoute] long productId,
            [FromQuery] long? tenantId = null)
        {
            try
            {
                var stats = await _reviewService.GetReviewStats(productId, tenantId);

                return Ok(new ApiResult
                {
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review stats for product {ProductId}", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Create a product review
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="request">Review request</param>
        /// <returns>Created review</returns>
        [Authorize]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> CreateReview(
            [FromRoute] long productId,
            [FromBody] CreateProductReviewRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Invalid request parameters"
                    });
                }

                // SECURITY: identity is derived from the JWT only — never trust request body UserId/TenantId.
                if (!TryGetUserId(out var userId))
                {
                    return Unauthorized(new ApiResult { Exception = "User ID not found in token" });
                }

                if (!TryGetTenantId(out var tenantId))
                {
                    return BadRequest(new ApiResult { Exception = "Tenant ID not found in token" });
                }

                // Ensure productId matches the route
                request.ProductId = productId;

                var review = await _reviewService.CreateReview(request, userId, tenantId);

                return Ok(new ApiResult
                {
                    Data = review
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for product {ProductId}", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Update a product review
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="reviewId">Review ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated review</returns>
        [Authorize]
        [HttpPut("{reviewId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> UpdateReview(
            [FromRoute] long productId,
            [FromRoute] long reviewId,
            [FromBody] UpdateProductReviewRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Invalid request parameters"
                    });
                }

                // SECURITY: identity is derived from the JWT only — never trust request body UserId/TenantId.
                if (!TryGetUserId(out var userId))
                {
                    return Unauthorized(new ApiResult { Exception = "User ID not found in token" });
                }

                if (!TryGetTenantId(out var tenantId))
                {
                    return BadRequest(new ApiResult { Exception = "Tenant ID not found in token" });
                }

                // Ensure reviewId matches the route
                request.ReviewId = reviewId;

                var review = await _reviewService.UpdateReview(request, userId, tenantId);

                return Ok(new ApiResult
                {
                    Data = review
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review {ReviewId} not found or not owned by caller on update", reviewId);
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", reviewId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Delete a product review
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="reviewId">Review ID</param>
        /// <returns>Success status</returns>
        [Authorize]
        [HttpDelete("{reviewId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> DeleteReview(
            [FromRoute] long productId,
            [FromRoute] long reviewId)
        {
            try
            {
                // SECURITY: identity is derived from the JWT only.
                if (!TryGetUserId(out var userId))
                {
                    return Unauthorized(new ApiResult { Exception = "User ID not found in token" });
                }

                if (!TryGetTenantId(out var tenantId))
                {
                    return BadRequest(new ApiResult { Exception = "Tenant ID not found in token" });
                }

                await _reviewService.DeleteReview(reviewId, userId, tenantId);

                return Ok(new ApiResult
                {
                    Data = new { success = true, message = "Review deleted successfully" }
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review {ReviewId} not found or not owned by caller on delete", reviewId);
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }

        /// <summary>
        /// Mark a review as helpful
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="reviewId">Review ID</param>
        /// <returns>Updated helpful count</returns>
        [Authorize]
        [HttpPost("{reviewId}/helpful")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        public async Task<IActionResult> MarkReviewHelpful(
            [FromRoute] long productId,
            [FromRoute] long reviewId)
        {
            try
            {
                // SECURITY: identity is derived from the JWT only.
                if (!TryGetUserId(out var userId))
                {
                    return Unauthorized(new ApiResult { Exception = "User ID not found in token" });
                }

                var helpfulCount = await _reviewService.MarkReviewHelpful(reviewId, userId);

                return Ok(new ApiResult
                {
                    Data = new { helpful = helpfulCount }
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review {ReviewId} not found on mark-helpful", reviewId);
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking review {ReviewId} as helpful", reviewId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = GenericErrorMessage
                });
            }
        }
    }
}

