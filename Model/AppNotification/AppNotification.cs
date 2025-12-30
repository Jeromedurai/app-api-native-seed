using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.AppNotification
{
    [Table("XC_INAPP_NOTIFICATION")]
    public class AppNotification : TnBase
    {
        [JsonProperty("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("userId")]
        public long? UserId { get; set; }

        [JsonProperty("roleId")]
        public string RoleId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fromUser")]
        public long? FromUser { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
