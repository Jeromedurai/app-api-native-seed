using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Util;

namespace Tenant.Query.Model.AppNotification
{
    public class NotificationMessage
    {
        [JsonProperty("messageId"), Key]
        public long MessageId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("createdOn"), JsonConverter(typeof(TnUtil.Date.XcDateTimeConverter_AwsDateTF))]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("viewedOn"), JsonConverter(typeof(TnUtil.Date.XcDateTimeConverter_AwsDateTF))]
        public DateTime? ViewedOn { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonIgnore]
        public int Count { get; set; }
    }
}
