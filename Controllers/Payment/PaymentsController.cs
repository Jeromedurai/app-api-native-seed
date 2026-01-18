using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Controller;
using Tenant.API.Base.Model;
using Tenant.Query.Service.Product;

namespace Tenant.Query.Controllers.Payment
{
    [Route("api/1.0/payments")]
    public class PaymentsController : TnBaseController<Service.Product.ProductService>
    {
        #region Initialize the value
        Service.Product.ProductService productService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public PaymentsController(ProductService service, IConfiguration configuration, ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {
            this.productService = service;
        }

        /// <summary>
        /// Create Razorpay order and returns a hosted checkout URL for redirect-based payment flow
        /// </summary>
        /// <param name="request">Razorpay hosted checkout request</param>
        /// <returns>Razorpay hosted checkout response with checkout URL</returns>
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("create-razorpay-hosted-checkout")]
        public async Task<IActionResult> CreateRazorpayHostedCheckout([FromBody] Model.Order.RazorpayHostedCheckoutRequest request)
        {
            try
            {
                // Validate request is not null
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request cannot be null", error = "Request body is required" });
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    var errorMessage = string.Join("; ", errors);
                    return BadRequest(new { success = false, message = errorMessage, error = "Validation failed" });
                }

                // Validate amount
                const long minAmount = 100; // Minimum: 100 paise (₹1)
                const long maxAmount = 10000000; // Maximum: 10,000,000 paise (₹1,00,000)
                
                if (request.Amount <= 0)
                {
                    return BadRequest(new { success = false, message = "Amount must be greater than 0", error = "Invalid amount" });
                }

                if (request.Amount < minAmount)
                {
                    return BadRequest(new { success = false, message = $"Amount must be at least {minAmount} paise (₹{minAmount / 100})", error = "Amount too small" });
                }

                if (request.Amount > maxAmount)
                {
                    return BadRequest(new { success = false, message = $"Amount cannot exceed {maxAmount / 100} rupees", error = "Amount too large" });
                }

                // Validate currency
                if (string.IsNullOrWhiteSpace(request.Currency))
                {
                    return BadRequest(new { success = false, message = "Currency is required", error = "Missing currency" });
                }

                if (request.Currency.Length != 3)
                {
                    return BadRequest(new { success = false, message = "Currency must be a valid 3-character code (e.g., INR)", error = "Invalid currency format" });
                }

                // Validate currency is INR (as per requirement)
                var currencyUpper = request.Currency.ToUpperInvariant();
                if (currencyUpper != "INR")
                {
                    return BadRequest(new { success = false, message = "Currency must be INR for Indian market", error = "Only INR currency is supported" });
                }

                // Validate return URL
                if (string.IsNullOrWhiteSpace(request.ReturnUrl))
                {
                    return BadRequest(new { success = false, message = "Return URL is required", error = "Missing return URL" });
                }

                if (!Uri.TryCreate(request.ReturnUrl, UriKind.Absolute, out Uri returnUri))
                {
                    return BadRequest(new { success = false, message = "Return URL must be a valid URL", error = "Invalid return URL format" });
                }

                // Validate cancel URL
                if (string.IsNullOrWhiteSpace(request.CancelUrl))
                {
                    return BadRequest(new { success = false, message = "Cancel URL is required", error = "Missing cancel URL" });
                }

                if (!Uri.TryCreate(request.CancelUrl, UriKind.Absolute, out Uri cancelUri))
                {
                    return BadRequest(new { success = false, message = "Cancel URL must be a valid URL", error = "Invalid cancel URL format" });
                }

                // Validate URLs against whitelist and HTTPS requirement
                var allowedDomains = Configuration["Razorpay:AllowedDomains"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) 
                    ?? new[] { "localhost:3000", "yourdomain.com", "www.yourdomain.com" };
                
                var isProduction = Configuration["Environment"]?.Equals("Production", StringComparison.OrdinalIgnoreCase) == true;
                
                // Validate return URL domain
                var returnDomain = $"{returnUri.Host}{(returnUri.IsDefaultPort ? "" : $":{returnUri.Port}")}";
                if (!allowedDomains.Any(d => returnDomain.Equals(d.Trim(), StringComparison.OrdinalIgnoreCase) || returnDomain.EndsWith($".{d.Trim()}", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new { success = false, message = "Return URL domain is not in the allowed whitelist", error = "Unauthorized return URL domain" });
                }

                // Validate cancel URL domain
                var cancelDomain = $"{cancelUri.Host}{(cancelUri.IsDefaultPort ? "" : $":{cancelUri.Port}")}";
                if (!allowedDomains.Any(d => cancelDomain.Equals(d.Trim(), StringComparison.OrdinalIgnoreCase) || cancelDomain.EndsWith($".{d.Trim()}", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new { success = false, message = "Cancel URL domain is not in the allowed whitelist", error = "Unauthorized cancel URL domain" });
                }

                // Validate HTTPS in production
                if (isProduction)
                {
                    if (returnUri.Scheme != "https")
                    {
                        return BadRequest(new { success = false, message = "Return URL must use HTTPS in production", error = "HTTPS required for return URL" });
                    }

                    if (cancelUri.Scheme != "https")
                    {
                        return BadRequest(new { success = false, message = "Cancel URL must use HTTPS in production", error = "HTTPS required for cancel URL" });
                    }
                }

                // Validate receipt format if provided
                if (!string.IsNullOrWhiteSpace(request.Receipt))
                {
                    if (request.Receipt.Length > 255)
                    {
                        return BadRequest(new { success = false, message = "Receipt cannot exceed 255 characters", error = "Receipt too long" });
                    }
                }

                // Validate optional UserId if provided
                if (request.UserId.HasValue)
                {
                    if (request.UserId.Value <= 0)
                    {
                        return BadRequest(new { success = false, message = "User ID must be greater than 0 if provided", error = "Invalid user ID" });
                    }

                    // Verify user exists in database
                    try
                    {
                        // var userService = HttpContext.RequestServices.GetService(typeof(Service.User.UserService)) as Service.User.UserService;
                        // if (userService != null)
                        // {
                        //     var userProfile = await userService.GetUserProfile(request.UserId.Value);
                        //     if (userProfile == null)
                        //     {
                        //         return BadRequest(new { success = false, message = "User not found or inactive", error = "User validation failed" });
                        //     }
                        // }
                    }
                    catch (KeyNotFoundException)
                    {
                        return BadRequest(new { success = false, message = "User not found or inactive", error = "User validation failed" });
                    }
                    catch (System.Exception ex)
                    {
                        // Log error but don't fail validation if service is unavailable
                        this.Logger.LogWarning($"User validation error: {ex.Message}");
                    }
                }

                // Validate customer email if provided
                if (!string.IsNullOrWhiteSpace(request.CustomerEmail) && !System.Text.RegularExpressions.Regex.IsMatch(request.CustomerEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return BadRequest(new { success = false, message = "Invalid email format", error = "Invalid customer email" });
                }

                // // Validate cart hash if provided (optional but recommended)
                // if (!string.IsNullOrWhiteSpace(request.CartHash) && request.UserId.HasValue)
                // {
                //     try
                //     {
                //         // Get cart items for the user
                //         var cartRequest = new Model.ProductCart.GetCartRequest
                //         {
                //             UserId = request.UserId.Value
                //         };
                //         var cartResponse = await this.productService.GetUserCart(cartRequest);
                        
                //         if (cartResponse != null && cartResponse.Items != null && cartResponse.Items.Any())
                //         {
                //             // Generate hash from cart items
                //             var cartData = string.Join("|", cartResponse.Items.OrderBy(x => x.Product.ProductId)
                //                 .Select(x => $"{x.Product.ProductId}:{x.Quantity}:{x.Product.Price}"));
                //             var cartHashBytes = System.Security.Cryptography.SHA256.Create()
                //                 .ComputeHash(System.Text.Encoding.UTF8.GetBytes(cartData));
                //             var generatedHash = Convert.ToHexString(cartHashBytes).ToLowerInvariant();

                //             // Compare with provided hash
                //             if (!request.CartHash.Equals(generatedHash, StringComparison.OrdinalIgnoreCase))
                //             {
                //                 return BadRequest(new { success = false, message = "Cart hash verification failed. Cart may have been tampered with.", error = "Cart hash mismatch" });
                //             }
                //         }
                //     }
                //     catch (System.Exception ex)
                //     {
                //         // Log error but don't fail validation if cart service is unavailable
                //         this.Logger.LogWarning($"Cart hash verification error: {ex.Message}");
                //     }
                // }

                // Call service to create Razorpay hosted checkout
                var result = await this.Service.CreateRazorpayHostedCheckout(request);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Failed to create Razorpay checkout", error = "Service returned null" });
                }

                // Return response in the format specified
                var response = new
                {
                    success = true,
                    data = new
                {
                    orderId = result.OrderId,
                        checkoutUrl = result.CheckoutUrl,
                    amount = result.Amount,
                        timestamp = result.Timestamp,
                        sessionToken = result.SessionToken
                    },
                    message = "Checkout URL generated successfully"
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message, error = "Invalid argument" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message, error = "Invalid operation" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while creating the Razorpay checkout. Please try again later.", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify Razorpay payment
        /// </summary>
        /// <param name="request">Payment verification request</param>
        /// <returns>Payment verification response</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("verify-razorpay-payment")]
        public async Task<IActionResult> VerifyRazorpayPayment([FromBody] Model.Order.RazorpayPaymentVerificationRequest request)
        {
            try
            {
                // Validate request is not null
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    var errorMessage = string.Join("; ", errors);
                    return BadRequest(new ApiResult { Exception = errorMessage });
                }

                // Validate Order ID
                if (string.IsNullOrWhiteSpace(request.OrderId))
                {
                    return BadRequest(new ApiResult { Exception = "Order ID is required" });
                }

                // Validate Order ID format (should start with "order_")
                // We use Razorpay Orders API, not Payment Links, so order ID must be "order_xxx"
                if (!request.OrderId.StartsWith("order_", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiResult { Exception = "Order ID must start with 'order_'" });
                }

                if (request.OrderId.Length < 10 || request.OrderId.Length > 255)
                {
                    return BadRequest(new ApiResult { Exception = "Order ID must be between 10 and 255 characters" });
                }

                // Validate Payment ID
                if (string.IsNullOrWhiteSpace(request.PaymentId))
                {
                    return BadRequest(new ApiResult { Exception = "Payment ID is required" });
                }

                // Validate Payment ID format (should start with "pay_")
                if (!request.PaymentId.StartsWith("pay_", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiResult { Exception = "Payment ID must start with 'pay_'" });
                }

                if (request.PaymentId.Length < 8 || request.PaymentId.Length > 255)
                {
                    return BadRequest(new ApiResult { Exception = "Payment ID must be between 8 and 255 characters" });
                }

                // Validate Signature
                if (string.IsNullOrWhiteSpace(request.Signature))
                {
                    return BadRequest(new ApiResult { Exception = "Signature is required" });
                }

                if (request.Signature.Length < 10 || request.Signature.Length > 500)
                {
                    return BadRequest(new ApiResult { Exception = "Signature must be between 10 and 500 characters" });
                }

                // Validate optional UserId if provided
                if (request.UserId.HasValue && request.UserId.Value <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "User ID must be greater than 0 if provided" });
                }

                // Validate optional TenantId if provided
                if (request.TenantId.HasValue && request.TenantId.Value <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Tenant ID must be greater than 0 if provided" });
                }

                // Validate optional OrderIdInternal if provided
                if (request.OrderIdInternal.HasValue && request.OrderIdInternal.Value <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Internal Order ID must be greater than 0 if provided" });
                }

                // Call service to verify payment
                var result = await this.Service.VerifyRazorpayPayment(request);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "Failed to verify payment" });
                }

                // Return response in the format specified
                var response = new
                {
                    success = result.Success,
                    message = result.Message
                };

                if (result.Success)
                {
                    return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = response });
                }
                else
                {
                    return BadRequest(new ApiResult { Data = response });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while verifying the payment. Please try again later." });
            }
        }

        /// <summary>
        /// Verify Razorpay payment and update order status
        /// </summary>
        /// <param name="request">Payment verification request</param>
        /// <returns>Payment verification response</returns>
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("verify-razorpay-payments")]
        public async Task<IActionResult> VerifyRazorpayPayments([FromBody] Model.Order.VerifyRazorpayPaymentsRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    var errorMessage = string.Join("; ", errors);
                    return BadRequest(new ApiResult { Exception = errorMessage });
                }

                // Validate Order ID
                if (string.IsNullOrWhiteSpace(request.OrderId))
                {
                    return BadRequest(new ApiResult { Exception = "Order ID is required" });
                }

                // Validate Order ID format (should start with "order_" or "plink_")
                if (!request.OrderId.StartsWith("order_", StringComparison.OrdinalIgnoreCase) &&
                    !request.OrderId.StartsWith("plink_", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiResult { Exception = "Order ID must start with 'order_' or 'plink_'" });
                }

                // Validate Payment ID
                if (string.IsNullOrWhiteSpace(request.PaymentId))
                {
                    return BadRequest(new ApiResult { Exception = "Payment ID is required" });
                }

                // Validate Payment ID format (should start with "pay_")
                if (!request.PaymentId.StartsWith("pay_", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiResult { Exception = "Payment ID must start with 'pay_'" });
                }

                // Validate Signature
                if (string.IsNullOrWhiteSpace(request.Signature))
                {
                    return BadRequest(new ApiResult { Exception = "Signature is required" });
                }

                // Call service to verify payment
                var result = await this.Service.VerifyRazorpayPayments(request);

                if (result == null)
                {
                    return NotFound(new ApiResult { Exception = "Payment verification failed" });
                }

                // Return success response
                return Ok(new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while verifying the payment. Please try again later." });
            }
        }

        /// <summary>
        /// Mark Razorpay payment as failed or cancelled
        /// </summary>
        /// <param name="request">Mark payment failed request</param>
        /// <returns>Mark payment failed response</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("mark-payment-failed")]
        public async Task<IActionResult> MarkPaymentFailed([FromBody] Model.Order.MarkPaymentFailedRequest request)
        {
            try
            {
                // Validate request is not null
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    var errorMessage = string.Join("; ", errors);
                    return BadRequest(new ApiResult { Exception = errorMessage });
                }

                // Validate Razorpay Order ID
                if (string.IsNullOrWhiteSpace(request.RazorpayOrderId))
                {
                    return BadRequest(new ApiResult { Exception = "Razorpay Order ID is required" });
                }

                // Validate Reason
                if (string.IsNullOrWhiteSpace(request.Reason) || 
                    (request.Reason != "cancelled" && request.Reason != "failed"))
                {
                    return BadRequest(new ApiResult { Exception = "Reason must be either 'cancelled' or 'failed'" });
                }

                // Call service to mark payment as failed
                var result = await this.Service.MarkPaymentFailed(request);

                if (result == null)
                {
                    return NotFound(new ApiResult { Exception = "Payment not found or could not be marked as failed" });
                }

                // Return success response
                return Ok(new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while marking payment as failed. Please try again later." });
            }
        }

        /// <summary>
        /// Get payment status for a Razorpay order
        /// </summary>
        /// <param name="razorpayOrderId">Razorpay Order ID</param>
        /// <returns>Payment status response</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpGet]
        [Route("payment-status/{razorpayOrderId}")]
        public async Task<IActionResult> GetPaymentStatus(string razorpayOrderId)
        {
            try
            {
                // Validate Razorpay Order ID
                if (string.IsNullOrWhiteSpace(razorpayOrderId))
                {
                    return BadRequest(new ApiResult { Exception = "Razorpay Order ID is required" });
                }

                // Call service to get payment status
                var result = await this.Service.GetPaymentStatus(razorpayOrderId);

                if (result == null)
                {
                    return NotFound(new ApiResult { Exception = "Payment status not found" });
                }

                // Return success response
                return Ok(new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = "An error occurred while retrieving payment status. Please try again later." });
            }
        }
    }
}

