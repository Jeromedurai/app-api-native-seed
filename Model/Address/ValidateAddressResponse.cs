using Newtonsoft.Json;

namespace Tenant.Query.Model.Address
{
    public class ValidateAddressResponse
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("validationMessage")]
        public string ValidationMessage { get; set; }
    }
}

