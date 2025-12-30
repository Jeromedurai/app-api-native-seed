using System;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for updating user profile request
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// User's full name (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string Name { get; set; }

        /// <summary>
        /// User's phone number (optional)
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
        public string Phone { get; set; }

        /// <summary>
        /// Date of birth (optional)
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gender (optional)
        /// </summary>
        [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters")]
        public string Gender { get; set; }

        /// <summary>
        /// User bio/description (optional)
        /// </summary>
        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        public string Bio { get; set; }

        /// <summary>
        /// Personal website URL (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Website URL cannot exceed 255 characters")]
        public string Website { get; set; }

        /// <summary>
        /// Company name (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Company name cannot exceed 255 characters")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Job title (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Job title cannot exceed 255 characters")]
        public string JobTitle { get; set; }

        /// <summary>
        /// Country (optional)
        /// </summary>
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string Country { get; set; }

        /// <summary>
        /// City (optional)
        /// </summary>
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        /// <summary>
        /// State/Province (optional)
        /// </summary>
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; }

        /// <summary>
        /// Postal/ZIP code (optional)
        /// </summary>
        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Timezone (optional)
        /// </summary>
        [StringLength(100, ErrorMessage = "Timezone cannot exceed 100 characters")]
        public string Timezone { get; set; }

        /// <summary>
        /// Preferred language (optional)
        /// </summary>
        [StringLength(10, ErrorMessage = "Language code cannot exceed 10 characters")]
        public string Language { get; set; }

        /// <summary>
        /// Preferred contact method (optional)
        /// </summary>
        [StringLength(50, ErrorMessage = "Preferred contact method cannot exceed 50 characters")]
        public string PreferredContactMethod { get; set; }

        /// <summary>
        /// Address information (optional)
        /// </summary>
        public AddressUpdateInfo Address { get; set; }
    }

    /// <summary>
    /// Model for address information in profile update
    /// </summary>
    public class AddressUpdateInfo
    {
        /// <summary>
        /// Street address
        /// </summary>
        [StringLength(255, ErrorMessage = "Street address cannot exceed 255 characters")]
        public string Street { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        /// <summary>
        /// State/Province
        /// </summary>
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; }

        /// <summary>
        /// ZIP/Postal code
        /// </summary>
        [StringLength(20, ErrorMessage = "ZIP code cannot exceed 20 characters")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string Country { get; set; }

        /// <summary>
        /// Address type (default: Home)
        /// </summary>
        [StringLength(50, ErrorMessage = "Address type cannot exceed 50 characters")]
        public string AddressType { get; set; } = "Home";

        /// <summary>
        /// Whether to update existing address or create new one (default: true)
        /// </summary>
        public bool UpdateIfExists { get; set; } = true;
    }
}
