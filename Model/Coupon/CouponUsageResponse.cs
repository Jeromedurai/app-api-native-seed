using System;
using Newtonsoft.Json;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for coupon usage response
    /// </summary>
    public class CouponUsageResponse
    {
        [JsonProperty("usageId")]
        public long UsageId { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("userId")]
        public long? UserId { get; set; }

        [JsonProperty("discountAmount")]
        public decimal DiscountAmount { get; set; }

        [JsonProperty("orderAmount")]
        public decimal OrderAmount { get; set; }

        [JsonProperty("usedAt")]
        public DateTime UsedAt { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }
    }

    /// <summary>
    /// Model for coupon usage statistics
    /// </summary>
    public class CouponUsageStatistics
    {
        [JsonProperty("totalUsage")]
        public int TotalUsage { get; set; }

        [JsonProperty("totalDiscountGiven")]
        public decimal TotalDiscountGiven { get; set; }

        [JsonProperty("totalOrderAmount")]
        public decimal TotalOrderAmount { get; set; }
    }
}

