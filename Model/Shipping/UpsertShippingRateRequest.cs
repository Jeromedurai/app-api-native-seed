using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Shipping
{
    public class UpsertShippingRateRequest
    {
        // 0 or null => INSERT a new rate; otherwise UPDATE the existing row.
        public long ShippingRateId { get; set; }

        [Required(ErrorMessage = "Tenant ID is required")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Product type is required")]
        public string ProductType { get; set; }            // 'Seed' or 'Plant'

        public string StateCode { get; set; }              // null/blank = all other states

        [Required(ErrorMessage = "Courier type is required")]
        public string CourierType { get; set; }            // 'Postal' or 'Other'

        [Range(0, double.MaxValue, ErrorMessage = "Base charge must be non-negative")]
        public decimal BaseCharge { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Per-unit charge must be non-negative")]
        public decimal PerUnitCharge { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Min charge must be non-negative")]
        public decimal MinCharge { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Max charge must be non-negative")]
        public decimal? MaxCharge { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Free shipping threshold must be non-negative")]
        public decimal? FreeShippingThreshold { get; set; }

        public bool Active { get; set; } = true;
    }
}
