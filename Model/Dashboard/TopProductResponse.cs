namespace Tenant.Query.Model.Dashboard
{
    /// <summary>
    /// Top product response
    /// </summary>
    public class TopProductResponse
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Total quantity sold
        /// </summary>
        public int TotalQuantitySold { get; set; }

        /// <summary>
        /// Total revenue
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Order count
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        public int StockQuantity { get; set; }
    }
}

