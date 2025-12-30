using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.User
{
    [Table("XC_ROLE_MASTER")]
    public class Role : TnBase
    {
        [JsonProperty("guid"), Key]
        public string Guid { get; set; }
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
        [JsonProperty("roleDescription")]
        public string RoleDescription { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("authorityLevel")]
        public Byte AuthorityLevel { get; set; }
        //[JsonIgnore,NotMapped]
        //public List<UserRole> UsreRoles { get; set; }

        [JsonProperty("created"), Column("CREATED_ON")]
        public DateTime Created { get; set; }

        [JsonProperty("lastModified"), Column("MODIFIED_ON")]
        public DateTime LastModified { get; set; }

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
