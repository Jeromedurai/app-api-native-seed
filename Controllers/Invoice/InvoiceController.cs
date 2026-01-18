using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.API.Base.Controller;
using Tenant.Query.Model.Order;
using Tenant.Query.Service.Invoice;
using Tenant.Query.Service.Product;

namespace Tenant.Query.Controllers.Invoice
{
    [Route("api/1.0/invoice")]
    [ApiController]
    public class InvoiceController : TnBaseController<ProductService>
    {
        private readonly ProductService _productService;
        private readonly InvoiceService _invoiceService;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public InvoiceController(
            ProductService productService,
            InvoiceService invoiceService,
            IConfiguration configuration,
            ILoggerFactory loggerFactory) : base(productService, configuration, loggerFactory)
        {
            _productService = productService;
            _invoiceService = invoiceService;
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Download invoice PDF for an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>PDF file</returns>
        [Authorize]
        [HttpGet("{orderId}/download")]
        [SwaggerResponse(StatusCodes.Status200OK, "Invoice PDF", typeof(FileResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public async Task<IActionResult> DownloadInvoice(long orderId, [FromQuery] long userId)
        {
                    var logger = _loggerFactory.CreateLogger<InvoiceController>();
            
            try
            {
                logger.LogInformation($"Invoice download request: OrderId={orderId}, UserId={userId}");

                if (userId <= 0)
                {
                    logger.LogWarning($"Invalid User ID: {userId}");
                    return BadRequest(new { message = "User ID is required" });
                }

                if (orderId <= 0)
                {
                    logger.LogWarning($"Invalid Order ID: {orderId}");
                    return BadRequest(new { message = "Order ID is required" });
                }

                // Get order details
                var orderRequest = new GetOrderByIdRequest
                {
                    OrderId = orderId,
                    UserId = userId,
                    IncludeItems = true,
                    IncludeAddress = true,
                    IncludePayment = true
                };

                logger.LogInformation($"Fetching order details for OrderId={orderId}, UserId={userId}");

                GetOrderByIdResponse order;
                try
                {
                    order = await _productService.GetOrderById(orderRequest);
                }
                catch (KeyNotFoundException ex)
                {
                    logger.LogWarning($"Order not found: OrderId={orderId}, UserId={userId}, Error={ex.Message}");
                    return NotFound(new { message = "Order not found or does not belong to user" });
                }
                catch (ArgumentException ex)
                {
                    logger.LogWarning($"Invalid request: {ex.Message}");
                    return BadRequest(new { message = ex.Message });
                }

                if (order == null)
                {
                    logger.LogWarning($"Order is null: OrderId={orderId}, UserId={userId}");
                    return NotFound(new { message = "Order not found" });
                }

                logger.LogInformation($"Order found: OrderNumber={order.OrderNumber}, ItemsCount={order.Items?.Count ?? 0}");

                // Generate PDF (order already contains items)
                logger.LogInformation($"Generating PDF for OrderId={orderId}");
                byte[] pdfBytes;
                try
                {
                    pdfBytes = _invoiceService.GenerateInvoicePdf(order);
                }
                catch (Exception pdfEx)
                {
                    logger.LogError(pdfEx, $"Error generating PDF for OrderId={orderId}: {pdfEx.Message}");
                    logger.LogError($"Inner exception: {pdfEx.InnerException?.Message}");
                    logger.LogError($"Stack trace: {pdfEx.StackTrace}");
                    return StatusCode(500, new { message = "Error generating invoice PDF", error = pdfEx.Message, details = pdfEx.InnerException?.Message });
                }

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    logger.LogError($"Generated PDF is empty for OrderId={orderId}");
                    return StatusCode(500, new { message = "Generated PDF is empty" });
                }

                logger.LogInformation($"PDF generated successfully: Size={pdfBytes.Length} bytes, OrderId={orderId}");

                // Return PDF file
                var fileName = $"Invoice_{order.OrderNumber ?? orderId.ToString()}_{DateTime.UtcNow:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning($"Order not found: OrderId={orderId}, UserId={userId}, Error={ex.Message}");
                return NotFound(new { message = "Order not found or does not belong to user", error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning($"Invalid argument: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unexpected error generating invoice for OrderId={orderId}, UserId={userId}");
                logger.LogError($"Exception type: {ex.GetType().Name}");
                logger.LogError($"Exception message: {ex.Message}");
                logger.LogError($"Inner exception: {ex.InnerException?.Message}");
                logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error generating invoice", error = ex.Message, details = ex.InnerException?.Message });
            }
        }
    }
}
