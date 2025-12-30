using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.AppNotification
{
    [Table("XC_APPCONFIG")]
    public class AppConfig : TnBase
    {
        [JsonProperty("id"), Column("ID")]
        public long Id { get; set; }

        [JsonProperty("configKey"), Column("CONFIGKEY")]
        public string ConfigKey { get; set; }

        [JsonProperty("configValue"), Column("CONFIGVALUE")]
        public string ConfigValue { get; set; }
    }
}
