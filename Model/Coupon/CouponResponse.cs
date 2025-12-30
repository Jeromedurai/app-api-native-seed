using System;
using Newtonsoft.Json;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for coupon response
    /// </summary>
    public class CouponResponse
    {
        [JsonProperty("couponId")]
        public long CouponId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("minAmount")]
        public decimal? MinAmount { get; set; }

        [JsonProperty("maxDiscount")]
        public decimal? MaxDiscount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("usageLimit")]
        public int? UsageLimit { get; set; }

        [JsonProperty("usageLimitPerUser")]
        public int? UsageLimitPerUser { get; set; }

        [JsonProperty("usageCount")]
        public int UsageCount { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("tenantId")]
        public long TenantId { get; set; }

        [JsonProperty("createdBy")]
        public long? CreatedBy { get; set; }

        [JsonProperty("updatedBy")]
        public long? UpdatedBy { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } // 'active', 'expired', 'upcoming'
    }
}

