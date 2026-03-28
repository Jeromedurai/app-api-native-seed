using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    public class StockNotificationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int TenantId { get; set; }
    }
}
