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
        /// Update a product review
        /// </summary>
        public async Task<ProductReviewResponse> UpdateReview(UpdateProductReviewRequest request, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Updating review {request.ReviewId} by user {userId}");

                // Validate rating if provided
                if (request.Rating.HasValue && (request.Rating.Value < 1 || request.Rating.Value > 5))
                {
                    throw new ArgumentException("Rating must be between 1 and 5");
                }

                return await _reviewRepository.UpdateReview(request, userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error updating review: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Delete a product review
        /// </summary>
        public async Task<bool> DeleteReview(long reviewId, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Deleting review {reviewId} by user {userId}");

                return await _reviewRepository.DeleteReview(reviewId, userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error deleting review: {ex.Message}", ex);
                throw;
            }
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
    }
}

