using Newtonsoft.Json;

namespace Tenant.Query.Model.Shipping
{
    public class StateResponse
    {
        [JsonProperty("stateId")]
        public long StateId { get; set; }

        [JsonProperty("stateCode")]
        public string StateCode { get; set; }

        [JsonProperty("stateName")]
        public string StateName { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("orderBy")]
        public int OrderBy { get; set; }
    }
}

