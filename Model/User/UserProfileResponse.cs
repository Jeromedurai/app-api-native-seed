using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user profile data
    /// </summary>
    public class UserProfileData
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>   
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's full name
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Whether email is verified
        /// </summary>
        public bool EmailVerified { get; set; }

        /// <summary>
        /// Whether phone is verified
        /// </summary>
        public bool PhoneVerified { get; set; }

        /// <summary>
        /// Account creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update date
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Last logout date
        /// </summary>
        public DateTime? LastLogout { get; set; }

        /// <summary>
        /// Profile picture URL
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// User's timezone
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Preferred language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State/Province
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Postal/ZIP code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// User bio/description
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Personal website
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Job title
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Preferred contact method
        /// </summary>
        public string PreferredContactMethod { get; set; }

        /// <summary>
        /// Notification settings JSON
        /// </summary>
        public string NotificationSettings { get; set; }

        /// <summary>
        /// Privacy settings JSON
        /// </summary>
        public string PrivacySettings { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleInfo> Roles { get; set; } = new List<UserRoleInfo>();

        /// <summary>
        /// User addresses
        /// </summary>
        public List<UserAddressInfo> Addresses { get; set; } = new List<UserAddressInfo>();

        /// <summary>
        /// User preferences
        /// </summary>
        public List<UserPreferenceInfo> Preferences { get; set; } = new List<UserPreferenceInfo>();

        /// <summary>
        /// User statistics
        /// </summary>
        public UserStatistics Statistics { get; set; } = new UserStatistics();
    }

    /// <summary>
    /// Model for user address information
    /// </summary>
    public class UserAddressInfo
    {
        /// <summary>
        /// Address ID
        /// </summary>
        public long AddressId { get; set; }

        /// <summary>
        /// Address type (e.g., Home, Work, Billing, Shipping)
        /// </summary>
        public string AddressType { get; set; }

        /// <summary>
        /// Street address
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State/Province
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Postal/ZIP code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Whether this is the default address
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Model for user preference information
    /// </summary>
    public class UserPreferenceInfo
    {
        /// <summary>
        /// Preference key/name
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Preference value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Preference type (e.g., string, boolean, number)
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Model for user statistics
    /// </summary>
    public class UserStatistics
    {
        /// <summary>
        /// Number of logins in the last 12 months
        /// </summary>
        public int LoginCount { get; set; }

        /// <summary>
        /// Days since last activity
        /// </summary>
        public int DaysSinceLastActivity { get; set; }

        /// <summary>
        /// Profile completion percentage (0-100)
        /// </summary>
        public int ProfileCompletion { get; set; }
    }
}
