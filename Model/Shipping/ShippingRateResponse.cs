using System;

namespace Tenant.Query.Model.Shipping
{
    public class ShippingRateResponse
    {
        public long ShippingRateId { get; set; }
        public long TenantId { get; set; }
        public string ProductType { get; set; }            // 'Seed' or 'Plant'
        public string StateCode { get; set; }              // null = all other states
        public string CourierType { get; set; }            // 'Postal' or 'Other'
        public decimal BaseCharge { get; set; }
        public decimal PerUnitCharge { get; set; }
        public decimal MinCharge { get; set; }
        public decimal? MaxCharge { get; set; }
        public decimal? FreeShippingThreshold { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
