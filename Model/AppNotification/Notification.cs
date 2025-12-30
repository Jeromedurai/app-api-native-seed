using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tenant.Query.Model.AppNotification
{
    public class Notification
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("messages")]
        public List<Model.AppNotification.NotificationMessage> Messages { get; set; }
    }
}
