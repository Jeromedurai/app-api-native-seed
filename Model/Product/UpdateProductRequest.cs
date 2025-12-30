using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    public class UpdateProductRequest
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductDescription { get; set; }

        [Required]
        public string ProductCode { get; set; }

        public string FullDescription { get; set; }
        public string Specification { get; set; }
        public string Story { get; set; }
        public int PackQuantity { get; set; } = 1;
        public int Quantity { get; set; }
        public int Total { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Category { get; set; }

        public int Rating { get; set; }
        public bool Active { get; set; } = true;
        public int Trending { get; set; }
        public int UserBuyCount { get; set; }
        public int Return { get; set; }
        public bool BestSeller { get; set; }
        public int DeliveryDate { get; set; }
        public string Offer { get; set; }
        public int OrderBy { get; set; }
        public long UserId { get; set; }
    }
}
