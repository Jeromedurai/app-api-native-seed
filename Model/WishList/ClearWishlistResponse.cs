using System;

namespace Tenant.Query.Model.WishList
{
    /// <summary>
    /// Model for clear wishlist response
    /// </summary>
    public class ClearWishlistResponse
    {
        /// <summary>
        /// User ID whose wishlist was cleared
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Number of unique items that were cleared
        /// </summary>
        public int ClearedItemCount { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time when the wishlist was cleared
        /// </summary>
        public DateTime ClearedDate { get; set; }

        /// <summary>
        /// Whether items were permanently deleted (true) or soft deleted (false)
        /// </summary>
        public bool WasHardDelete { get; set; }
    }
}
