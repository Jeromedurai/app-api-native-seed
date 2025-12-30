using System;
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
        private readonly ProductReviewService _reviewService;
        public ILogger Logger { get; set; }
        #endregion

        public ProductReviewsController(
            ProductReviewService reviewService,
            ILoggerFactory loggerFactory)
        {
            _reviewService = reviewService;
            this.Logger = loggerFactory.CreateLogger<ProductReviewsController>();
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
                Logger.LogError($"Error getting reviews: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                Logger.LogError($"Error getting review stats: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Create a product review
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="request">Review request</param>
        /// <returns>Created review</returns>
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

                // Get user ID from claims first, then fallback to request body
                long userId = 0;
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out userId))
                {
                    // Use from claims
                }
                else if (request.UserId > 0)
                {
                    // Use from request body
                    userId = request.UserId;
                }
                else
                {
                    return Unauthorized(new ApiResult
                    {
                        Exception = "User ID not found in token or request"
                    });
                }

                // Get tenant ID from claims first, then fallback to request body
                long tenantId = 0;
                var tenantIdClaim = User.FindFirst("TenantId");
                if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out tenantId))
                {
                    // Use from claims
                }
                else if (request.TenantId > 0)
                {
                    // Use from request body
                    tenantId = request.TenantId;
                }
                else
                {
                    return BadRequest(new ApiResult
                    {
                        Exception = "Tenant ID not found in token or request"
                    });
                }

                // Ensure productId matches
                request.ProductId = productId;

                var review = await _reviewService.CreateReview(request, userId, tenantId);

                return Ok(new ApiResult
                {
                    Data = review
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating review: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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

                // Get user ID from claims first, then fallback to request body
                long userId = 0;
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out userId))
                {
                    // Use from claims
                }
                else if (request.UserId > 0)
                {
                    // Use from request body
                    userId = request.UserId;
                }
                else
                {
                    return Unauthorized(new ApiResult
                    {
                        Exception = "User ID not found in token or request"
                    });
                }

                // Ensure reviewId matches
                request.ReviewId = reviewId;

                var review = await _reviewService.UpdateReview(request, userId);

                return Ok(new ApiResult
                {
                    Data = review
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating review: {ex.Message}", ex);
                
                if (ex.Message.Contains("not found") || ex.Message.Contains("permission"))
                {
                    return NotFound(new ApiResult
                    {
                        Exception = ex.Message
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                // Get user ID from claims
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
                {
                    return Unauthorized(new ApiResult
                    {
                        Exception = "User ID not found in token"
                    });
                }

                await _reviewService.DeleteReview(reviewId, userId);

                return Ok(new ApiResult
                {
                    Data = new { success = true, message = "Review deleted successfully" }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting review: {ex.Message}", ex);
                
                if (ex.Message.Contains("not found") || ex.Message.Contains("permission"))
                {
                    return NotFound(new ApiResult
                    {
                        Exception = ex.Message
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
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
                // Get user ID from claims
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
                {
                    return Unauthorized(new ApiResult
                    {
                        Exception = "User ID not found in token"
                    });
                }

                var helpfulCount = await _reviewService.MarkReviewHelpful(reviewId, userId);

                return Ok(new ApiResult
                {
                    Data = new { helpful = helpfulCount }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error marking review as helpful: {ex.Message}", ex);
                
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new ApiResult
                    {
                        Exception = ex.Message
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult
                {
                    Exception = ex.Message
                });
            }
        }
    }
}

