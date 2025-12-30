using Newtonsoft.Json;

namespace Tenant.Query.Model.Response.ProductMaster
{
    public class ProductImages
    {
        [JsonProperty("imageId")]
        public long ImageId { get; set; }

        [JsonProperty("poster")]
        public string Poster { get; set; }

        [JsonProperty("main")]
        public bool Main { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("orderBy")]
        public long OrderBy { get; set; }
    }
}
