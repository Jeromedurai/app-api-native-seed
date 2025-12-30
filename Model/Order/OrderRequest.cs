using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Tenant.Query.Model.Order
{
    public class OrderRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("shippingAddressId")]
        public int ShippingAddressId { get; set; }

        [JsonProperty("billingAddressId")]
        public int BillingAddressId { get; set; }

        [JsonProperty("items")]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }
    }
}
