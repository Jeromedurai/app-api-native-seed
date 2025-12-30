using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Constant;
using Tenant.Query.Model.Product;
using Tenant.Query.Uitility;

namespace Tenant.Query.Repository.Product
{
    public class ProductReviewRepository
    {
        #region Variables
        private readonly Sa.Common.ADO.DataAccess.DataAccess _dataAccess;
        private readonly ProductContext _dbContext;
        private string dbConnectionString = string.Empty;
        public ILogger Logger { get; set; }
        #endregion

        public ProductReviewRepository(ProductContext dbContext, ILoggerFactory loggerFactory, Sa.Common.ADO.DataAccess.DataAccess dataAccess)
        {
            _dbContext = dbContext;
            dbConnectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            _dataAccess = dataAccess;  // Use the injected instance
            this.Logger = loggerFactory.CreateLogger<ProductReviewRepository>();
        }

        /// <summary>
        /// Create a product review
        /// </summary>
        public async Task<ProductReviewResponse> CreateReview(CreateProductReviewRequest request, long userId, long tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Creating review for product {request.ProductId} by user {userId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_CREATE_PRODUCT_REVIEW,
                    request.ProductId,
                    userId,
                    tenantId,
                    request.Rating,
                    request.ReviewTitle ?? (object)DBNull.Value,
                    request.ReviewText ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to create review");
                }

                return MapToReviewResponse(result.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error creating review: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Getting reviews for product {productId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_PRODUCT_REVIEWS,
                    productId,
                    tenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    return new List<ProductReviewResponse>();
                }

                return result.Tables[0].AsEnumerable()
                    .Select(row => MapToReviewResponse(row))
                    .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting reviews: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Getting review stats for product {productId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_PRODUCT_REVIEW_STATS,
                    productId,
                    tenantId ?? (object)DBNull.Value
                ));

                var stats = new ProductReviewStatsResponse();

                if (result != null && result.Tables.Count > 0)
                {
                    // Get rating distribution (first table)
                    if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in result.Tables[0].Rows)
                        {
                            var rating = Convert.ToInt32(row["Rating"]);
                            var count = Convert.ToInt32(row["Count"]);
                            if (stats.RatingDistribution.ContainsKey(rating))
                            {
                                stats.RatingDistribution[rating] = count;
                            }
                        }
                    }

                    // Get total reviews and average rating (second table)
                    if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                    {
                        var statsRow = result.Tables[1].Rows[0];
                        stats.TotalReviews = Convert.ToInt32(statsRow["TotalReviews"]);
                        stats.AverageRating = Convert.ToDecimal(statsRow["AverageRating"]);
                    }
                }

                return stats;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting review stats: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Updating review {request.ReviewId} by user {userId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_UPDATE_PRODUCT_REVIEW,
                    request.ReviewId,
                    userId,
                    request.Rating ?? (object)DBNull.Value,
                    request.ReviewTitle ?? (object)DBNull.Value,
                    request.ReviewText ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to update review");
                }

                return MapToReviewResponse(result.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error updating review: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Delete a product review (soft delete)
        /// </summary>
        public async Task<bool> DeleteReview(long reviewId, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Deleting review {reviewId} by user {userId}");

                await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_DELETE_PRODUCT_REVIEW,
                    reviewId,
                    userId
                ));

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error deleting review: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Repository: Marking review {reviewId} as helpful by user {userId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_MARK_REVIEW_HELPFUL,
                    reviewId,
                    userId
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to mark review as helpful");
                }

                return Convert.ToInt32(result.Tables[0].Rows[0]["Helpful"]);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error marking review as helpful: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Map DataRow to ProductReviewResponse
        /// </summary>
        private ProductReviewResponse MapToReviewResponse(DataRow row)
        {
            return new ProductReviewResponse
            {
                ReviewId = Convert.ToInt64(row["ReviewId"]),
                ProductId = Convert.ToInt64(row["ProductId"]),
                UserId = Convert.ToInt64(row["UserId"]),
                UserName = row["UserName"]?.ToString() ?? "",
                UserEmail = row["UserEmail"]?.ToString() ?? "",
                Rating = Convert.ToInt32(row["Rating"]),
                Title = row["Title"]?.ToString() ?? "",
                Comment = row["Comment"]?.ToString() ?? "",
                Verified = Convert.ToBoolean(row["Verified"]),
                Helpful = Convert.ToInt32(row["Helpful"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
            };
        }
    }
}

