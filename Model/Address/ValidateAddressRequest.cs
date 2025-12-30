using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Address
{
    public class ValidateAddressRequest
    {
        [Required(ErrorMessage = "Street address is required")]
        [StringLength(255, ErrorMessage = "Street address cannot exceed 255 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string Country { get; set; } = "IN";
    }
}

