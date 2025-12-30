using Newtonsoft.Json;

namespace Tenant.Query.Model.AppNotification
{
    public class EdiEnrollmentRequest
    {
        [JsonProperty("toAddress")]
        public string ToAddress { get; set; }
        [JsonProperty("fromAddress")]
        public string FromAddress { get; set; }
        [JsonProperty("tenantName")]
        public string TenantName { get; set; }
        [JsonProperty("integrationType")]
        public string IntegrationType { get; set; }
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }
        [JsonProperty("singleLocation")]
        public SingleLocation SingleLocation { get; set; }
    }

    public class SingleLocation
    {
        [JsonProperty("customerNumber")]
        public string CustomerNumber { get; set; }
        [JsonProperty("operatingCompany")]
        public string OperatingCompany { get; set; }
        [JsonProperty("accountManager")]
        public string AccountManager { get; set; }
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }
        [JsonProperty("businessLegalName")]
        public string BusinessLegalName { get; set; }
        [JsonProperty("businessAddress")]
        public string BusinessAddress { get; set; }
        [JsonProperty("restaurantContactName")]
        public string RestaurantContactName { get; set; }
        [JsonProperty("restaurantContactEmailAddress")]
        public string RestaurantContactEmailAddress { get; set; }
        [JsonProperty("restaurantContactPhoneNumber")]
        public string RestaurantContactPhoneNumber { get; set; }
    }
}
