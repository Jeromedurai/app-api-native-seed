using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tenant.Query.Model.User
{
    public class UserDetails
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
                
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("systemAdmin")]
        public bool SystemAdmin { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        public List<Address> Addresses { get; internal set; }
    }


    public class Address
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("addressType")]
        public string AddressType { get; set; } // 'Shipping', 'Billing', or 'Both'

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }
    }

    public class SpUserMasterList
    {
        [JsonProperty("userId"), Column("USER_ID")]
        public int UserId { get; set; }

        [JsonProperty("firstName"), Column("FIRST_NAME")]
        public string FirstName { get; set; }

        [JsonProperty("lastName"), Column("LAST_NAME")]
        public string LastName { get; set; }

        [JsonProperty("email"), Column("Email")]
        public string Email { get; set; }

        [JsonProperty("phone"), Column("Phone")]
        public string Phone { get; set; }

        [JsonProperty("systemAdmin"), Column("SYSTEM_ADMIN")]
        public bool SystemAdmin { get; set; }

        [JsonProperty("created_at"), Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("id"), Column("Id")]
        public int Id { get; set; }

        [JsonProperty("addressType"), Column("AddressType")]
        public string AddressType { get; set; }

        [JsonProperty("street"), Column("Street")]
        public string Street { get; set; }

        [JsonProperty("city"), Column("City")]
        public string City { get; set; }

        [JsonProperty("state"), Column("State")]
        public string State { get; set; }

        [JsonProperty("postalCode"), Column("PostalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("country"), Column("Country")]
        public string Country { get; set; }

        [JsonProperty("isDefault"), Column("IsDefault")]
        public bool IsDefault { get; set; }
    }
}
