using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.User
{
    [Table("XC_USER")]
    public class User : TnBase
    {
        #region Properties

        [JsonProperty("guid")]
        public override string Guid { get; set; }

        [JsonProperty("userId"), Column("USER_ID")]
        public string UserId { get; set; }

        //[JsonProperty("tenantId"), Column("TENANT_ID")]
        //public override string TenantId { get; set; }

        //[JsonProperty("locationId"), Column("LOCATION_ID")]
        //public override string LocationId { get; set; }

        [JsonProperty("userName"), Column("USER_NAME")]
        public string UserName { get; set; }

        [JsonProperty("firstName"), Column("FIRST_NAME")]
        public string FirstName { get; set; }

        [JsonProperty("middleName"), Column("MIDDLE_NAME")]
        public string MiddleName { get; set; }

        [JsonProperty("lastName"), Column("LAST_NAME")]
        public string LastName { get; set; }

        [JsonProperty("status"), NotMapped]
        public string Status { get; set; }

        [JsonProperty("lastLogin"), Column("LAST_LOGIN")]
        public DateTime? LastLogin { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        //[JsonProperty("created"), Column("CREATED_ON")]
        //public override DateTime Created { get; set; }

        //[JsonIgnore, NotMapped]
        //public override string CreatedBy { get; set; }

        //[JsonProperty("lastModified"), Column("MODIFIED_ON")]
        //public override DateTime LastModified { get; set; }

        //[JsonIgnore, NotMapped]
        //public override string LastModifiedBy { get; set; }

        [JsonProperty("systemUser"), Column("IS_SYSTEM_USER")]
        public bool? SystemUser { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(this.MiddleName) ? $"{this.LastName}, {this.FirstName}" : $"{this.LastName}, {this.FirstName} {this.MiddleName}");
            }
        }

        [JsonProperty("userRoles")]
        public List<UserRole> UserRoles { get; set; }

        [JsonIgnore, NotMapped]
        public override string LastModifiedBy { get; set; }
        [JsonIgnore, NotMapped]
        public override string CreatedBy { get; set; }
        [JsonIgnore, NotMapped]
        public override string TenantId { get; set; }
        [JsonIgnore, NotMapped]
        public override long Id { get; set; }
        #endregion
    }
}
