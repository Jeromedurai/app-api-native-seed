using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    public sealed class Response
    {
        [JsonProperty("invoiceItems")]
        public List<InvoiceItemMappingDetails> InvoiceItemMappingDetails { get; set; }

        [JsonProperty("totalRowCount"), Column("TotalRowCount")]
        public int? TotalRowCount { get; set; }
    }

    public class InvoiceItemMappingDetails
    {
        [JsonProperty("rowNumber"), Column("RowNumber")]
        public int RowNumber { get; set; }

        [JsonProperty("productId"), Column("ProductId"), Key]
        public long? ProductId { get; set; }

        [JsonProperty("vendorProductCode"), Column("VendorProductCode")]
        public string VendorProductCode { get; set; }

        [JsonProperty("productDescription"), Column("ProductDescription")]
        public string ProductDescription { get; set; }

        [JsonProperty("extractPreference"), Column("ExtractPreference")]
        public int? ExtractPreference { get; set; }

        [JsonProperty("vendorId"), Column("VendorId")]
        public long? VendorId { get; set; }

        [JsonProperty("vendorName"), Column("VendorName")]
        public string VendorName { get; set; }

        [JsonProperty("sizePack"), Column("SizePack")]
        public string SizePack { get; set; }

        [JsonProperty("unitOfMeasurement"), Column("UnitOfMeasurement")]
        public string UnitOfMeasurement { get; set; }

        [JsonProperty("tenantVendorId"), Column("TenantVendorId")]
        public long? TenantVendorId { get; set; }

        [JsonProperty("productExtentedId"), Column("ProductExtendedId")]
        public long? ProductExtentedId { get; set; }
    }
    public class SpInvoiceItemMappingResponse
    {
        [JsonProperty("rowNumber"), Column("RowNumber")]
        public int RowNumber { get; set; }

        [JsonProperty("productId"), Column("ProductId"), Key]
        public long? ProductId { get; set; }

        [JsonProperty("vendorProductCode"), Column("VendorProductCode")]
        public string VendorProductCode { get; set; }

        [JsonProperty("productDescription"), Column("ProductDescription")]
        public string ProductDescription { get; set; }

        [JsonProperty("extractPreference"), Column("ExtractPreference")]
        public int? ExtractPreference { get; set; }

        [JsonProperty("vendorId"), Column("VendorId")]
        public long? VendorId { get; set; }

        [JsonProperty("vendorName"), Column("VendorName")]
        public string VendorName { get; set; }

        [JsonProperty("sizePack"), Column("SizePack")]
        public string SizePack { get; set; }

        [JsonProperty("unitOfMeasurement"), Column("UnitOfMeasurement")]
        public string UnitOfMeasurement { get; set; }

        [JsonProperty("tenantVendorId"), Column("TenantVendorId")]
        public long? TenantVendorId { get; set; }

        [JsonProperty("productExtentedId"), Column("ProductExtendedId")]
        public long? ProductExtentedId { get; set; }

        [JsonProperty("totalRowCount"), Column("TotalRowCount")]
        public int? TotalRowCount { get; set; }
    }
}
