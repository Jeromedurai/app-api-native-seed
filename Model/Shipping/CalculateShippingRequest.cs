using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Shipping
{
    public class CalculateShippingRequest
    {
        [Required(ErrorMessage = "Tenant ID is required")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Product type is required")]
        public string ProductType { get; set; } // 'Seed' or 'Plant'

        [Required(ErrorMessage = "State code is required")]
        public string StateCode { get; set; }

        [Required(ErrorMessage = "Courier type is required")]
        public string CourierType { get; set; } // 'Postal' or 'Other'

        [Required(ErrorMessage = "Subtotal is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Subtotal must be non-negative")]
        public decimal Subtotal { get; set; }

        public int TotalQuantity { get; set; } = 1;
    }
}

