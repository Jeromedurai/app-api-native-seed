using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Order
{
    public class OrderResponse
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("items")]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
