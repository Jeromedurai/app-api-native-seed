using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Shipping
{
    public class CalculateMixedShippingRequest
    {
        [Required(ErrorMessage = "Tenant ID is required")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "State code is required")]
        public string StateCode { get; set; }

        [Required(ErrorMessage = "Courier type is required")]
        public string CourierType { get; set; } // 'Postal' or 'Other'

        [Required(ErrorMessage = "Seed subtotal is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Seed subtotal must be non-negative")]
        public decimal SeedSubtotal { get; set; }

        [Required(ErrorMessage = "Plant subtotal is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Plant subtotal must be non-negative")]
        public decimal PlantSubtotal { get; set; }

        public int SeedQuantity { get; set; } = 0;

        public int PlantQuantity { get; set; } = 0;

        [Required(ErrorMessage = "Total subtotal is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total subtotal must be non-negative")]
        public decimal TotalSubtotal { get; set; }
    }
}

