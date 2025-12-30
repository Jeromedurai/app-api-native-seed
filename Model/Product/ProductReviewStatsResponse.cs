using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Response model for product review statistics
    /// </summary>
    public class ProductReviewStatsResponse
    {
        /// <summary>
        /// Total number of reviews
        /// </summary>
        public int TotalReviews { get; set; }

        /// <summary>
        /// Average rating
        /// </summary>
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Rating distribution (rating -> count)
        /// </summary>
        public Dictionary<int, int> RatingDistribution { get; set; }

        public ProductReviewStatsResponse()
        {
            RatingDistribution = new Dictionary<int, int>
            {
                { 5, 0 },
                { 4, 0 },
                { 3, 0 },
                { 2, 0 },
                { 1, 0 }
            };
        }
    }
}

