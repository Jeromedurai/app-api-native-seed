using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.User
{
    [Table("XC_USER_ROLES")]
    public class UserRole : TnBase
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("roleId")]
        public string RoleId { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("created"), Column("CREATED_ON")]
        public override DateTime Created { get; set; }

        [JsonProperty("lastModified"), Column("MODIFIED_ON")]
        public override DateTime LastModified { get; set; }

        [JsonProperty("roles"), NotMapped]
        public Model.User.Role Role { get; set; }

        [JsonIgnore, NotMapped]
        public override string LastModifiedBy { get; set; }
        [JsonIgnore, NotMapped]
        public override string CreatedBy { get; set; }
        [JsonIgnore, NotMapped]
        public override string TenantId { get; set; }

        [JsonIgnore, NotMapped]
        public override long Id { get; set; }
    }
}
