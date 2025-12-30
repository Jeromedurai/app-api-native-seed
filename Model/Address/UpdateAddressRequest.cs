using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Address
{
    public class UpdateAddressRequest
    {
        [Required(ErrorMessage = "Address ID is required")]
        public long AddressId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        [StringLength(50, ErrorMessage = "Address type cannot exceed 50 characters")]
        public string AddressType { get; set; }

        [StringLength(255, ErrorMessage = "Street address cannot exceed 255 characters")]
        public string Street { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string Country { get; set; }

        public bool? IsDefault { get; set; }

        public bool? Active { get; set; }
    }
}

