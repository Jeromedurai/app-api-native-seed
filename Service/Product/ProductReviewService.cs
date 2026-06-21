using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tenant.Query.Model.Product;
using Tenant.Query.Repository.Product;

namespace Tenant.Query.Service.Product
{
    public class ProductReviewService
    {
        #region Variables
        private readonly ProductReviewRepository _reviewRepository;
        public ILogger Logger { get; set; }
        #endregion

        public ProductReviewService(
            ProductReviewRepository reviewRepository,
            ILoggerFactory loggerFactory)
        {
            _reviewRepository = reviewRepository;
            this.Logger = loggerFactory.CreateLogger<ProductReviewService>();
        }

        /// <summary>
        /// Create a product review
        /// </summary>
        public async Task<ProductReviewResponse> CreateReview(CreateProductReviewRequest request, long userId, long tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Creating review for product {request.ProductId} by user {userId}");

                // Validate request
                if (request.Rating < 1 || request.Rating > 5)
                {
                    throw new ArgumentException("Rating must be between 1 and 5");
                }

                return await _reviewRepository.CreateReview(request, userId, tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error creating review: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get reviews for a product
        /// </summary>
        public async Task<List<ProductReviewResponse>> GetReviews(long productId, long? tenantId = null)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting reviews for product {productId}");

                return await _reviewRepository.GetReviews(productId, tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting reviews: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get top reviews (rating >= minRating) for home page ticker
        /// </summary>
        public async Task<List<ProductReviewResponse>> GetTopReviews(long tenantId, int minRating = 4, int limit = 20)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting top reviews for tenant {tenantId}");
                return await _reviewRepository.GetTopReviews(tenantId, minRating, limit);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting top reviews: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get review statistics for a product
        /// </summary>
        public async Task<ProductReviewStatsResponse> GetReviewStats(long productId, long? tenantId = null)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting review stats for product {productId}");

                return await _reviewRepository.GetReviewStats(productId, tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting review stats: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Update a product review. Ownership is enforced in the SP by (ReviewId, UserId, TenantId);
        /// a not-found/permission failure surfaces as <see cref="KeyNotFoundException"/> (mapped to 404).
        /// </summary>
        public async Task<ProductReviewResponse> UpdateReview(UpdateProductReviewRequest request, long userId, long tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Updating review {request.ReviewId} by user {userId} (tenant {tenantId})");

                // Validate rating if provided
                if (request.Rating.HasValue && (request.Rating.Value < 1 || request.Rating.Value > 5))
                {
                    throw new ArgumentException("Rating must be between 1 and 5");
                }

                return await _reviewRepository.UpdateReview(request, userId, tenantId);
            }
            catch (Exception ex) when (IsNotFoundOrPermission(ex))
            {
                this.Logger.LogWarning($"Service: Update denied for review {request.ReviewId} by user {userId} (tenant {tenantId})");
                throw new KeyNotFoundException("Review not found or you do not have permission to update it");
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error updating review: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Delete a product review. Ownership is enforced in the SP by (ReviewId, UserId, TenantId);
        /// a not-found/permission failure surfaces as <see cref="KeyNotFoundException"/> (mapped to 404).
        /// </summary>
        public async Task<bool> DeleteReview(long reviewId, long userId, long tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Deleting review {reviewId} by user {userId} (tenant {tenantId})");

                return await _reviewRepository.DeleteReview(reviewId, userId, tenantId);
            }
            catch (Exception ex) when (IsNotFoundOrPermission(ex))
            {
                this.Logger.LogWarning($"Service: Delete denied for review {reviewId} by user {userId} (tenant {tenantId})");
                throw new KeyNotFoundException("Review not found or you do not have permission to delete it");
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error deleting review: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// True when the underlying SP raised the "not found / no permission" error for a review,
        /// so the caller can map it to a 404 instead of a 500. Already-typed
        /// <see cref="KeyNotFoundException"/> is passed through.
        /// </summary>
        private static bool IsNotFoundOrPermission(Exception ex)
        {
            if (ex is KeyNotFoundException) return true;
            var msg = ex.Message ?? string.Empty;
            return msg.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("permission", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Mark a review as helpful
        /// </summary>
        public async Task<int> MarkReviewHelpful(long reviewId, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Marking review {reviewId} as helpful by user {userId}");

                return await _reviewRepository.MarkReviewHelpful(reviewId, userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error marking review as helpful: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<AdminReviewListResponse> GetAllReviewsAdmin(long? tenantId, int page, int limit)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting all reviews for admin, tenantId={tenantId}, page={page}");
                return await _reviewRepository.GetAllReviewsAdmin(tenantId, page, limit);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting all reviews for admin: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<AdminReviewResponse> ToggleReviewActive(long reviewId, bool active)
        {
            try
            {
                this.Logger.LogInformation($"Service: Toggling review {reviewId} active={active}");
                return await _reviewRepository.ToggleReviewActive(reviewId, active);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error toggling review active: {ex.Message}", ex);
                throw;
            }
        }
    }
}

