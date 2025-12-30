using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tenant.Query.Model.User
{
    public class Filter
    {
        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("groups")]
        public string[] Groups { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("includeUserRole")]
        public bool IncludeUserRole { get; set; }

        [JsonProperty("includeSystemUser")]
        public bool IncludeSystemUser { get; set; }

        [JsonProperty("includeUserLocation")]
        public bool IncludeUserLocation { get; set; }
    }
}
