using Newtonsoft.Json;

namespace Tenant.Query.Model.Shipping
{
    public class CalculateShippingResponse
    {
        [JsonProperty("shippingCharge")]
        public decimal? ShippingCharge { get; set; }

        [JsonProperty("freeShipping")]
        public bool FreeShipping { get; set; }

        [JsonProperty("freeShippingThreshold")]
        public decimal? FreeShippingThreshold { get; set; }

        [JsonProperty("courierType")]
        public string CourierType { get; set; }
    }
}

