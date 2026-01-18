using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Tenant.Query.Model.Order;

namespace Tenant.Query.Service.Invoice
{
    /// <summary>
    /// Service for generating invoice PDFs
    /// </summary>
    public class InvoiceService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IConfiguration configuration, ILogger<InvoiceService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Generate PDF invoice for an order
        /// </summary>
        /// <param name="order">Order details</param>
        /// <returns>PDF byte array</returns>
        public byte[] GenerateInvoicePdf(GetOrderByIdResponse order)
        {
            try
            {
                if (order == null)
                {
                    _logger.LogError("Order is null in GenerateInvoicePdf");
                    throw new ArgumentNullException(nameof(order));
                }

                _logger.LogInformation($"Generating PDF for OrderId={order.OrderId}, OrderNumber={order.OrderNumber}");

                var companyName = _configuration["Invoice:CompanyName"] ?? "Xtrachef";
                var companyAddress = _configuration["Invoice:CompanyAddress"] ?? "";
                var companyPhone = _configuration["Invoice:CompanyPhone"] ?? "";
                var companyEmail = _configuration["Invoice:CompanyEmail"] ?? "";
                var companyGST = _configuration["Invoice:CompanyGST"] ?? "";

                // Parse shipping address
                var shippingAddress = ParseAddress(order.ShippingAddress);
                var customerName = GetCustomerName(shippingAddress);
                var customerAddress = FormatAddress(shippingAddress);

                _logger.LogInformation($"Customer: {customerName}, Items: {order.Items?.Count ?? 0}");

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header()
                            .Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text(companyName).FontSize(20).FontFamily(Fonts.Calibri).Bold().FontColor(Colors.Green.Darken3);
                                    if (!string.IsNullOrEmpty(companyAddress))
                                        column.Item().Text(companyAddress).FontSize(9);
                                    if (!string.IsNullOrEmpty(companyPhone))
                                        column.Item().Text($"Phone: {companyPhone}").FontSize(9);
                                    if (!string.IsNullOrEmpty(companyEmail))
                                        column.Item().Text($"Email: {companyEmail}").FontSize(9);
                                    if (!string.IsNullOrEmpty(companyGST))
                                        column.Item().Text($"GSTIN: {companyGST}").FontSize(9);
                                });

                                row.ConstantItem(100).AlignRight().Column(column =>
                                {
                                    column.Item().Text("INVOICE").FontSize(24).Bold().FontColor(Colors.Green.Darken3);
                                    column.Item().Text($"Invoice #: {order.OrderNumber}").FontSize(10);
                                    column.Item().Text($"Date: {order.CreatedAt:dd MMM yyyy}").FontSize(10);
                                    if (!string.IsNullOrEmpty(order.PaymentStatus))
                                        column.Item().Text($"Status: {order.PaymentStatus}").FontSize(10);
                                });
                            });

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                // Bill To section
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text("Bill To:").FontSize(11).Bold();
                                        col.Item().Text(customerName).FontSize(10);
                                        col.Item().Text(customerAddress).FontSize(9);
                                    });
                                });

                                column.Item().PaddingTop(10);

                                // Order Items Table
                                column.Item().Table(table =>
                                {
                                    // Header
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Item").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Qty").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Price").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Discount").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                                    });

                                    // Items
                                    var orderItems = order.Items ?? new List<OrderDetailItemInfo>();
                                    _logger.LogInformation($"Processing {orderItems.Count} order items");
                                    
                                    if (orderItems.Count == 0)
                                    {
                                        // Add a row indicating no items - use all 5 columns
                                        table.Cell().Element(CellStyle).Text("No items found").FontSize(10).FontColor(Colors.Grey.Medium);
                                        table.Cell().Element(CellStyle).Text("");
                                        table.Cell().Element(CellStyle).Text("");
                                        table.Cell().Element(CellStyle).Text("");
                                        table.Cell().Element(CellStyle).Text("");
                                    }
                                    else
                                    {
                                        foreach (var item in orderItems)
                                        {
                                            if (item == null) continue;
                                            
                                            table.Cell().Element(CellStyle).Column(column =>
                                            {
                                                column.Item().Text(item?.ProductName ?? "Unknown Product");
                                                if (!string.IsNullOrEmpty(item?.ProductCode))
                                                    column.Item().Text($"Code: {item.ProductCode}").FontSize(8).FontColor(Colors.Grey.Medium);
                                            });
                                            
                                            table.Cell().Element(CellStyle).AlignRight().Text((item?.Quantity ?? 0).ToString());
                                            table.Cell().Element(CellStyle).AlignRight().Text($"₹{(item?.Price ?? 0):F2}");
                                            table.Cell().Element(CellStyle).AlignRight().Text("₹0.00"); // Discount not in OrderItemDetail
                                            table.Cell().Element(CellStyle).AlignRight().Text($"₹{(item?.Total ?? 0):F2}");
                                        }
                                    }
                                });

                                column.Item().PaddingTop(20);

                                // Summary
                                column.Item().AlignRight().Column(summaryColumn =>
                                {
                                    summaryColumn.Item().Row(row =>
                                    {
                                        row.ConstantItem(100).Text("Subtotal:").FontSize(10);
                                        row.ConstantItem(100).AlignRight().Text($"₹{order.Subtotal:F2}").FontSize(10);
                                    });

                                    if (order.ShippingAmount > 0)
                                    {
                                        summaryColumn.Item().Row(row =>
                                        {
                                            row.ConstantItem(100).Text("Shipping:").FontSize(10);
                                            row.ConstantItem(100).AlignRight().Text($"₹{order.ShippingAmount:F2}").FontSize(10);
                                        });
                                    }

                                    if (order.TaxAmount > 0)
                                    {
                                        summaryColumn.Item().Row(row =>
                                        {
                                            row.ConstantItem(100).Text("Tax:").FontSize(10);
                                            row.ConstantItem(100).AlignRight().Text($"₹{order.TaxAmount:F2}").FontSize(10);
                                        });
                                    }

                                    if (order.DiscountAmount > 0)
                                    {
                                        summaryColumn.Item().Row(row =>
                                        {
                                            row.ConstantItem(100).Text("Discount:").FontSize(10).FontColor(Colors.Red.Medium);
                                            row.ConstantItem(100).AlignRight().Text($"-₹{order.DiscountAmount:F2}").FontSize(10).FontColor(Colors.Red.Medium);
                                        });
                                    }

                                    summaryColumn.Item().PaddingTop(5);
                                    summaryColumn.Item().Row(row =>
                                    {
                                        row.ConstantItem(100).Text("Total:").FontSize(12).Bold();
                                        row.ConstantItem(100).AlignRight().Text($"₹{order.TotalAmount:F2}").FontSize(12).Bold();
                                    });
                                });

                                column.Item().PaddingTop(20);

                                // Footer
                                column.Item().Text("Thank you for your business!").FontSize(10).Italic().AlignCenter();
                                if (!string.IsNullOrEmpty(order.Notes))
                                {
                                    column.Item().PaddingTop(10);
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Note: ").FontSize(9).FontColor(Colors.Grey.Medium);
                                        text.Span(order.Notes).FontSize(9).FontColor(Colors.Grey.Medium);
                                    });
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("This is a computer-generated invoice. No signature required.").FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                _logger.LogInformation($"PDF generated successfully: {pdfBytes.Length} bytes");
                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating invoice PDF for OrderId={order?.OrderId}");
                _logger.LogError($"Exception type: {ex.GetType().Name}");
                _logger.LogError($"Exception message: {ex.Message}");
                _logger.LogError($"Inner exception: {ex.InnerException?.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5)
                .PaddingHorizontal(5);
        }

        private object ParseAddress(string addressJson)
        {
            if (string.IsNullOrEmpty(addressJson))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<System.Dynamic.ExpandoObject>(addressJson);
            }
            catch
            {
                return null;
            }
        }

        private string FormatAddress(object address)
        {
            if (address == null)
                return "";

            var parts = new List<string>();
            
            try
            {
                var dict = address as IDictionary<string, object>;
                if (dict != null)
                {
                    if (dict.ContainsKey("Address1") && dict["Address1"] != null)
                        parts.Add(dict["Address1"].ToString());
                    if (dict.ContainsKey("Address2") && dict["Address2"] != null)
                        parts.Add(dict["Address2"].ToString());
                    if (dict.ContainsKey("City") && dict["City"] != null)
                        parts.Add(dict["City"].ToString());
                    if (dict.ContainsKey("State") && dict["State"] != null)
                        parts.Add(dict["State"].ToString());
                    if (dict.ContainsKey("ZipCode") && dict["ZipCode"] != null)
                        parts.Add(dict["ZipCode"].ToString());
                    if (dict.ContainsKey("Country") && dict["Country"] != null)
                        parts.Add(dict["Country"].ToString());
                }
            }
            catch
            {
                // If parsing fails, return empty
            }

            return string.Join(", ", parts);
        }

        private string GetCustomerName(object address)
        {
            if (address == null)
                return "Customer";

            try
            {
                var dict = address as IDictionary<string, object>;
                if (dict != null)
                {
                    var firstName = dict.ContainsKey("FirstName") ? dict["FirstName"]?.ToString() : "";
                    var lastName = dict.ContainsKey("LastName") ? dict["LastName"]?.ToString() : "";
                    var name = $"{firstName} {lastName}".Trim();
                    return string.IsNullOrEmpty(name) ? "Customer" : name;
                }
            }
            catch
            {
                // If parsing fails, return default
            }

            return "Customer";
        }
    }
}
