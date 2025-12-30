using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XtraChef.Content.Command.Model
{
    public class ChartDataList
    {
        [JsonProperty("reason")]
        public string REASON { get; set; }

        [JsonProperty("invoiceCount")]
        public Int64 INVOICE_COUNT { get; set; }

        [JsonProperty("locationId")]
        public Int64 LOCATION_ID { get; set; }

        [JsonProperty("locationName")]
        public Int64 LOCATION_NAME { get; set; }
    }
}
