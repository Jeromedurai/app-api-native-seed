using Newtonsoft.Json;

namespace Tenant.Query.Model.Coupon
{
    /// <summary>
    /// Model for coupon validation response
    /// </summary>
    public class ValidateCouponResponse
    {
        [JsonProperty("valid")]
        public bool Valid { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("couponId")]
        public long? CouponId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [JsonProperty("discountAmount")]
        public decimal? DiscountAmount { get; set; }
    }
}

