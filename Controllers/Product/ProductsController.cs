using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Controller;
using Tenant.API.Base.Model;
using Tenant.Query.Model.Product;
using Tenant.Query.Model.ProductCart;
using Tenant.Query.Model.WishList;
using Tenant.Query.Model.Settings;
using Tenant.Query.Service.Product;
using Exception = System.Exception;

namespace Tenant.Query.Controllers.Product
{
    [Route("api/1.0/products")]
    public class ProductsController : TnBaseController<Service.Product.ProductService>
    {
        #region Initialize the value
        Service.Product.ProductService productService;
        private const int MaxFileSize = 10 * 1024 * 1024; // 10MB
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public ProductsController(ProductService service, IConfiguration configuration, ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {
            this.productService = service;
        }

        #region New Endpoint 

        /// <summary>
        /// Upload image by productid
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("tenants/{tenantId}/upload")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<ActionResult<ImageResponseDto>> UploadImage([FromRoute] string tenantId, [FromForm] ImageUploadDto dto)
        {
            try
            {
                // Validation
                if (dto == null)
                    return BadRequest("Request data is missing");

                if (dto.File == null || dto.File.Length == 0)
                    return BadRequest("No file uploaded");

                if (dto.File.Length > MaxFileSize)
                    return BadRequest($"File size exceeds {MaxFileSize / (1024 * 1024)}MB limit");

                // Validate content type
                var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                var contentType = dto.File.ContentType?.ToLower();
                var extension = Path.GetExtension(dto.File.FileName)?.ToLower();
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                
                // Check content type or file extension
                bool isValidType = false;
                if (!string.IsNullOrEmpty(contentType) && allowedContentTypes.Contains(contentType))
                {
                    isValidType = true;
                }
                else if (!string.IsNullOrEmpty(extension) && validExtensions.Contains(extension))
                {
                    isValidType = true;
                }

                if (!isValidType)
                {
                    return BadRequest($"Invalid file type. Allowed types: JPEG, PNG, GIF, WebP. Received: {contentType ?? "unknown"}");
                }

                // Process image - read directly from the file stream
                byte[] imageData;
                try
                {
                    // Read the file directly into a byte array
                    using var fileStream = dto.File.OpenReadStream();
                    using var memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    
                    // Ensure we have the complete data
                    imageData = memoryStream.ToArray();
                    
                    // Verify we got the expected amount of data
                    if (imageData.Length != dto.File.Length)
                    {
                        return BadRequest($"File read incomplete. Expected: {dto.File.Length} bytes, Got: {imageData.Length} bytes");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error reading file: {ex.Message}");
                }

                if (imageData == null || imageData.Length == 0)
                    return BadRequest("File is empty or could not be read");

                // Validate minimum file size (should be at least a few bytes for a valid image)
                if (imageData.Length < 100)
                    return BadRequest("File is too small to be a valid image");

                // Check file signature as a preliminary check (informational only)
                bool hasValidSignature = false;
                string detectedFormat = "unknown";
                
                if (imageData.Length >= 3)
                {
                    // Check JPEG file signature (FF D8 FF)
                    if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
                    {
                        hasValidSignature = true;
                        detectedFormat = "JPEG";
                    }
                    // Check PNG file signature (89 50 4E 47)
                    else if (imageData.Length >= 4 && 
                            imageData[0] == 0x89 && imageData[1] == 0x50 && 
                            imageData[2] == 0x4E && imageData[3] == 0x47)
                    {
                        hasValidSignature = true;
                        detectedFormat = "PNG";
                    }
                    // Check GIF file signature (GIF87a or GIF89a)
                    else if (imageData.Length >= 6 && 
                            imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 && 
                            imageData[3] == 0x38 && (imageData[4] == 0x37 || imageData[4] == 0x39) && imageData[5] == 0x61)
                    {
                        hasValidSignature = true;
                        detectedFormat = "GIF";
                    }
                    // Check WebP signature (RIFF....WEBP)
                    else if (imageData.Length >= 12 &&
                            imageData[0] == 0x52 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x46 &&
                            imageData[8] == 0x57 && imageData[9] == 0x45 && imageData[10] == 0x42 && imageData[11] == 0x50)
                    {
                        hasValidSignature = true;
                        detectedFormat = "WebP";
                    }
                }

                // Validate with ImageSharp - it's the authoritative check
                // But if validation fails and we have a valid content type + reasonable file size, 
                // we'll allow it (some images might have minor issues but still be usable)
                bool isValidImage = productService.IsImageValid(imageData);
                
                if (!isValidImage)
                {
                    // If validation failed but we have:
                    // 1. Valid content type
                    // 2. Reasonable file size (>1KB)
                    // 3. File extension matches content type
                    // Then allow it anyway (ImageSharp might be too strict for some edge cases)
                    bool shouldAllowAnyway = false;
                    string reason = "";
                    
                    if (!string.IsNullOrEmpty(contentType) && allowedContentTypes.Contains(contentType))
                    {
                        if (imageData.Length > 1024) // At least 1KB
                        {
                            if (!string.IsNullOrEmpty(extension) && validExtensions.Contains(extension))
                            {
                                shouldAllowAnyway = true;
                                reason = "Validation failed but file has valid content type, extension, and reasonable size. Proceeding with upload.";
                            }
                        }
                    }
                    
                    if (!shouldAllowAnyway)
                    {
                        string signatureInfo = hasValidSignature 
                            ? $"Detected format: {detectedFormat}" 
                            : "File signature check: No recognized format signature found";
                        
                        return BadRequest($"Invalid image file. The file may be corrupted or in an unsupported format. {signatureInfo}. File size: {imageData.Length} bytes, Content-Type: {contentType ?? "unknown"}");
                    }
                    else
                    {
                        // Log warning but allow the upload
                        // You might want to add logging here if you have a logger available
                    }
                }

                // Create thumbnail if requested
                byte[] thumbnailData = null;
                if (dto.GenerateThumbnail)
                {
                    try
                    {
                        thumbnailData = await productService.CreateThumbnailAsync(
                            imageData,
                            dto.ThumbnailWidth ?? 200,
                            dto.ThumbnailHeight ?? 200);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                // Save to database
                var image = new ProductNewImage
                {
                    ProductId = dto.ProductId,
                    ImageName = Path.GetFileName(dto.File.FileName),
                    ContentType = productService.GetContentType(dto.File.FileName),
                    ImageData = imageData,
                    ThumbnailData = thumbnailData,
                    FileSize = (int)dto.File.Length,
                    CreatedAt = DateTime.UtcNow
                };

                long imageId = this.Service.AddImages(image);

                // Return response
                return Ok(new ImageResponseDto
                {
                    Id = imageId,
                    ImageName = image.ImageName,
                    ContentType = image.ContentType,
                    FileSize = image.FileSize,
                    CreatedAt = image.CreatedAt,
                    ImageUrl = Url.ActionLink("GetImage", "Products", new { id = imageId }),
                    ThumbnailUrl = thumbnailData != null
                        ? Url.ActionLink("GetThumbnail", "Products", new { id = imageId })
                        : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get image by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        // [ResponseCache(Duration = 86400)] // Cache for 1 day
        public async Task<IActionResult> GetImage(long id)
        {
            try
            {
                var image = await productService.GetImageAsync(id);

                if (image == null)
                    return NotFound();

                return File(image.ImageData, image.ContentType, image.ImageName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get product thumline image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}/thumbnail")]
        // [ResponseCache(Duration = 86400)]
        public async Task<IActionResult> GetThumbnail(long id)
        {
            try
            {
                var image = await productService.GetImageAsync(id); // Ensure GetImage is async

                if (image == null)
                    return NotFound();

                // Fallback to full image if thumbnail doesn't exist
                var data = image.ThumbnailData ?? image.ImageData;
                return File(data, image.ContentType, $"thumb_{image.ImageName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get product list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost]
        [Route("tenants/{tenantId}/get-product-list")]
        public IActionResult GetProductList([FromRoute] string tenantId, [FromBody] Model.Product.ProductPayload payload)
        {
            try
            {
                List<Model.Product.ProductItemList> productItemList = this.Service.GetProductList(tenantId, payload);


                    // Updated usage in the existing code
                    productItemList = productItemList ?? new List<ProductItemList>();
                    MapProductImages(productItemList);

                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = productItemList });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("tenants/{tenantId}")]
        public IActionResult GetValueByKey([FromRoute] string tenantId)
        {
            try
            {
                string key = "MAX_ITEMS_IN_CART"; // Example key, this could be a parameter
                string productItemList = this.Service.GetValueByKey(key);

                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = productItemList });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get application settings (admin)
        /// </summary>
        [HttpGet]
        [Route("settings")]
        public IActionResult GetAppSettings([FromQuery] long? tenantId = null, [FromQuery] long? userId = null)
        {
            try
            {
                var result = this.Service.GetAppSettings(tenantId, userId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Save application settings (admin)
        /// </summary>
        [HttpPost]
        [Route("settings")]
        public IActionResult SaveAppSettings([FromBody] SaveAppSettingsRequest request)
        {
            try
            {
                if (request == null || request.Settings == null || request.Settings.Count == 0)
                {
                    return BadRequest(new ApiResult { Exception = "Settings payload is required" });
                }

                var result = this.Service.SaveAppSettings(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get product sliders for dashboard (New, Sale, Recommended)
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="limit">Number of products per category (default: 8)</param>
        /// <returns>Products grouped by category</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpGet]
        [Route("tenants/{tenantId:long}/product-sliders")]
        public async Task<IActionResult> GetProductSliders([FromRoute] long tenantId, [FromQuery] int limit = 8)
        {
            try
            {
                if (limit < 1 || limit > 20)
                {
                    limit = 8;
                }

                // Get New Products (recently added, sorted by created date)
                var newProductsPayload = new ProductSearchPayload
                {
                    Page = 1,
                    Limit = limit,
                    SortBy = "created",
                    SortOrder = "desc",
                    Search = "",
                    MinPrice = null,
                    MaxPrice = null
                };
                var newProductsResult = await this.Service.SearchProductsAsync(tenantId.ToString(), newProductsPayload);

                // Get Sale Products (products with offers)
                var saleProductsPayload = new ProductSearchPayload
                {
                    Page = 1,
                    Limit = limit,
                    SortBy = "price",
                    SortOrder = "asc",
                    Search = "",
                    HasOffer = true,
                    MinPrice = null,
                    MaxPrice = null
                };
                var saleProductsResult = await this.Service.SearchProductsAsync(tenantId.ToString(), saleProductsPayload);

                // Get Recommended Products (best sellers / highly rated)
                var recommendedProductsPayload = new ProductSearchPayload
                {
                    Page = 1,
                    Limit = limit,
                    SortBy = "rating",
                    SortOrder = "desc",
                    Search = "",
                    BestSeller = true,
                    MinPrice = null,
                    MaxPrice = null
                };
                var recommendedProductsResult = await this.Service.SearchProductsAsync(tenantId.ToString(), recommendedProductsPayload);

                // Helper function to process images using Url.ActionLink for proper URL generation
                void ProcessProductImages(ProductSearchResponse result)
                {
                    if (result?.Products != null)
                    {
                        foreach (var product in result.Products)
                        {
                            if (product.Images != null)
                            {
                                foreach (var image in product.Images)
                                {
                                    // Generate proper URLs using Url.ActionLink
                                    image.ImageUrl = Url.ActionLink("GetImage", "Products", new { id = image.ImageId }) ?? "";
                                    image.ThumbnailUrl = Url.ActionLink("GetThumbnail", "Products", new { id = image.ImageId }) ?? "";
                                    if (string.IsNullOrEmpty(image.Poster))
                                    {
                                        image.Poster = image.ImageUrl;
                                    }
                                }
                            }
                        }
                    }
                }

                ProcessProductImages(newProductsResult);
                ProcessProductImages(saleProductsResult);
                ProcessProductImages(recommendedProductsResult);

                var result = new
                {
                    newProducts = newProductsResult?.Products ?? new List<ProductDetailItem>(),
                    saleProducts = saleProductsResult?.Products ?? new List<ProductDetailItem>(),
                    recommendedProducts = recommendedProductsResult?.Products ?? new List<ProductDetailItem>()
                };

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get featured products by category (new, sale, recommended)
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="category">Category type: new, sale, recommended</param>
        /// <param name="limit">Number of products (default: 10)</param>
        /// <returns>Featured products for specified category</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpGet]
        [Route("tenants/{tenantId:long}/featured-products")]
        public async Task<IActionResult> GetFeaturedProducts(
            [FromRoute] long tenantId, 
            [FromQuery] string category = "new", 
            [FromQuery] int limit = 10)
        {
            try
            {
                if (limit < 1 || limit > 50)
                {
                    limit = 10;
                }

                var validCategories = new[] { "new", "sale", "recommended" };
                if (!validCategories.Contains(category?.ToLower()))
                {
                    category = "new";
                }

                var payload = new ProductSearchPayload
                {
                    Page = 1,
                    Limit = limit,
                    Search = "",
                    MinPrice = null,
                    MaxPrice = null
                };

                // Configure payload based on category
                switch (category.ToLower())
                {
                    case "new":
                        payload.SortBy = "created";
                        payload.SortOrder = "desc";
                        break;
                    case "sale":
                        payload.SortBy = "price";
                        payload.SortOrder = "asc";
                        payload.HasOffer = true;
                        break;
                    case "recommended":
                        payload.SortBy = "rating";
                        payload.SortOrder = "desc";
                        payload.BestSeller = true;
                        break;
                }

                var searchResult = await this.Service.SearchProductsAsync(tenantId.ToString(), payload);

                // Generate full URLs for all product images using Url.ActionLink
                if (searchResult?.Products != null)
                {
                    foreach (var product in searchResult.Products)
                    {
                        if (product.Images != null)
                        {
                            foreach (var image in product.Images)
                            {
                                image.ImageUrl = Url.ActionLink("GetImage", "Products", new { id = image.ImageId }) ?? "";
                                image.ThumbnailUrl = Url.ActionLink("GetThumbnail", "Products", new { id = image.ImageId }) ?? "";
                                if (string.IsNullOrEmpty(image.Poster))
                                {
                                    image.Poster = image.ImageUrl;
                                }
                            }
                        }
                    }
                }

                var result = new
                {
                    products = searchResult?.Products ?? new List<ProductDetailItem>(),
                    category = category,
                    total = searchResult?.Pagination?.Total ?? 0
                };

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Generate image url
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public ImageResponseDto GetImages(ImageResponseDto image)
        {
            return new ImageResponseDto
            {
                Id = image.Id,
                ImageName = image.ImageName,
                ContentType = image.ContentType,
                FileSize = image.FileSize,
                CreatedAt = image.CreatedAt,
                ImageUrl = Url.ActionLink("GetImage", "Products", new { id = image.Id }),
                ThumbnailUrl = Url.ActionLink("GetThumbnail", "Products", new { id = image.Id })
            };
        }

        //// DELETE api/images/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteImage(long id)
        //{
        //    try
        //    {
        //        var image = await productService.DeleteImage(id);
        //        if (image == null)
        //            return NotFound();

        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        #endregion

        #region Crude endpoint        
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/add-product_category")]
        public IActionResult AddProductCategory([FromRoute] long tenantId, [FromBody] Model.Product.ProductCategoryPayload payload)
        {
            try
            {
                var result = this.Service.AddProductCategory(tenantId, payload);
                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
            finally
            {
            }
        }

        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/add-to-cart")]
        public async Task<IActionResult> UpsertCart(long tenantId, [FromBody] CartPayload payload)
        {
            try
            {
                ProductCartResponse result = await productService.UpsertCart(tenantId, payload);

                result.images = result.images.Select(GetImages).ToList();

                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
            finally
            {
            }
        }

        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpGet]
        [AllowAnonymous]
        [Route("tenants/{userId:long}/get-user-cart")]
        public IActionResult GeUserCart(long userId)
        {
            try
            {
                List<ProductCartResponse> productCartList = productService.GetUserCart(userId);

                productCartList = productCartList ?? new List<ProductCartResponse>();
                MapProductCartImages(productCartList);

                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = productCartList });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
            finally
            {
            }
        }

        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/add-to-wishlist")]
        public async Task<IActionResult> UpsertWishList(long tenantId, [FromBody] WishListPayload payload)
        {
            try
            {
                ProductWishListResponse result = await productService.UpsertWishList(tenantId, payload);

                result.images = result.images.Select(GetImages).ToList();

                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
            finally
            {
            }
        }



        /// <summary>
        /// Search products with advanced filtering and pagination
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpPost]
        [Route("tenants/{tenantId}/search-products")]
        public async Task<IActionResult> SearchProducts([FromRoute] string tenantId, [FromBody] ProductSearchPayload payload)
        {
            try
            {
                if (payload == null)
                {
                    return BadRequest(new ApiResult { Exception = "Payload cannot be null" });
                }

                // Validate pagination parameters
                if (payload.Page < 1)
                {
                    payload.Page = 1;
                }

                if (payload.Limit < 1 || payload.Limit > 100)
                {
                    payload.Limit = 10;
                }

                // Validate sort parameters
                var validSortFields = new[] { "productName", "price", "rating", "userBuyCount", "created" };
                if (!validSortFields.Contains(payload.SortBy.ToLower()))
                {
                    payload.SortBy = "created";
                }

                var validSortOrders = new[] { "asc", "desc" };
                if (!validSortOrders.Contains(payload.SortOrder.ToLower()))
                {
                    payload.SortOrder = "desc";
                }

                var result = await this.Service.SearchProductsAsync(tenantId, payload);

                // Generate full URLs for all product images
                if (result?.Products != null)
                {
                    foreach (var product in result.Products)
                    {
                        if (product.Images != null)
                        {
                            foreach (var image in product.Images)
                            {
                                // Generate full image URL and thumbnail URL
                                var request = HttpContext.Request;
                                var baseUrl = $"{request.Scheme}://{request.Host}";
                                image.ImageUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}";
                                image.ThumbnailUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}/thumbnail";
                                
                                // Set Poster to ImageUrl for backward compatibility
                                if (string.IsNullOrEmpty(image.Poster))
                                {
                                    image.Poster = image.ImageUrl;
                                }
                            }
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Enhanced product search with full-text, fuzzy matching, and advanced filters
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpPost]
        [Route("tenants/{tenantId}/search-products-enhanced")]
        public async Task<IActionResult> SearchProductsEnhanced([FromRoute] string tenantId, [FromBody] EnhancedProductSearchPayload payload)
        {
            try
            {
                if (payload == null)
                {
                    return BadRequest(new ApiResult { Exception = "Payload cannot be null" });
                }

                // Validate pagination parameters
                if (payload.Page < 1) payload.Page = 1;
                if (payload.Limit < 1 || payload.Limit > 100) payload.Limit = 10;

                // Validate sort parameters - include 'relevance' for enhanced search
                var validSortFields = new[] { "productName", "price", "rating", "userBuyCount", "created", "relevance" };
                if (!validSortFields.Contains(payload.SortBy?.ToLower() ?? "relevance"))
                {
                    payload.SortBy = "relevance";
                }

                var validSortOrders = new[] { "asc", "desc" };
                if (!validSortOrders.Contains(payload.SortOrder?.ToLower() ?? "desc"))
                {
                    payload.SortOrder = "desc";
                }

                var result = await this.Service.SearchProductsEnhancedAsync(tenantId, payload);

                // Generate full URLs for all product images
                if (result?.Products != null)
                {
                    foreach (var product in result.Products)
                    {
                        if (product.Images != null)
                        {
                            foreach (var image in product.Images)
                            {
                                var request = HttpContext.Request;
                                var baseUrl = $"{request.Scheme}://{request.Host}";
                                image.ImageUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}";
                                image.ThumbnailUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}/thumbnail";
                                if (string.IsNullOrEmpty(image.Poster))
                                {
                                    image.Poster = image.ImageUrl;
                                }
                            }
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get search suggestions for autocomplete
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpGet]
        [Route("tenants/{tenantId}/search/suggestions")]
        public IActionResult GetSearchSuggestions([FromRoute] long tenantId, [FromQuery] string query, [FromQuery] int limit = 8)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Ok(new ApiResult { Data = new SearchSuggestionResponse { Success = true, Suggestions = new List<SearchSuggestion>() } });
                }

                if (limit < 1 || limit > 20) limit = 8;

                var request = new SearchSuggestionRequest
                {
                    TenantId = tenantId,
                    Query = query.Trim(),
                    Limit = limit
                };

                var result = this.Service.GetSearchSuggestions(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get filtered product list for admin management
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="payload">Filter parameters</param>
        /// <returns>Filtered product list with pagination</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpPost]
        [Route("tenants/{tenantId:long}/get-product-list-filtered")]
        public IActionResult GetProductListFiltered([FromRoute] long tenantId, [FromBody] ProductListFilteredRequest payload)
        {
            try
            {
                if (payload == null)
                {
                    return BadRequest(new ApiResult { Exception = "Payload cannot be null" });
                }

                // Validate pagination parameters
                if (payload.Page < 1)
                {
                    payload.Page = 1;
                }

                if (payload.Limit < 1 || payload.Limit > 1000)
                {
                    payload.Limit = 10;
                }

                // Validate sort parameters
                var validSortFields = new[] { "productName", "price", "rating", "userBuyCount", "created" };
                if (!validSortFields.Contains(payload.SortBy?.ToLower()))
                {
                    payload.SortBy = "productName";
                }

                var validSortOrders = new[] { "asc", "desc" };
                if (!validSortOrders.Contains(payload.SortOrder?.ToLower()))
                {
                    payload.SortOrder = "ASC";
                }

                var result = this.Service.GetProductListFiltered(tenantId.ToString(), payload);

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get product images by product ID
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>List of product images</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpGet]
        [Route("products/{productId:long}/images")]
        public async Task<IActionResult> GetProductImages([FromRoute] long productId)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid product ID" });
                }

                var result = await this.Service.GetProductById(productId);

                if (result == null)
                {
                    return NotFound(new ApiResult { Exception = "Product not found" });
                }

                // Return only the images
                var images = result.Images ?? new List<ProductSearchImageInfo>();
                
                // Map to response format with image URLs
                var imageResponses = images.Select(img => new
                {
                    imageId = img.ImageId,
                    productId = productId,
                    imageUrl = Url.ActionLink("GetImage", "Products", new { id = img.ImageId }),
                    thumbnailUrl = Url.ActionLink("GetThumbnail", "Products", new { id = img.ImageId }),
                    main = img.Main,
                    active = img.Active,
                    orderBy = img.OrderBy,
                    created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    modified = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }).ToList();

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = imageResponses });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get product details by ID
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Product details with images</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpGet]
        [Route("{productId:long}")]
        public async Task<IActionResult> GetProductById([FromRoute] long productId)
        {
            try
            {
                var result = await this.Service.GetProductById(productId);

                if (result == null)
                {
                    return NotFound(new ApiResult { Exception = "Product not found" });
                }

                // Map image URLs
                if (result.Images != null && result.Images.Any())
                {
                    // Generate full URLs for all product images
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    
                    foreach (var image in result.Images)
                    {
                        image.ImageUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}";
                        image.ThumbnailUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}/thumbnail";
                        
                        // Set Poster to ImageUrl for backward compatibility
                        if (string.IsNullOrEmpty(image.Poster))
                        {
                            image.Poster = image.ImageUrl;
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Product details</param>
        /// <returns>Product ID</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/add-product")]
        public async Task<IActionResult> AddProduct([FromRoute] long tenantId, [FromBody] AddProductRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid request model" });
                }

                var productId = await this.Service.AddProduct(tenantId, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = "Product added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Product details</param>
        /// <returns>Success message</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/update-product")]
        public async Task<IActionResult> UpdateProduct([FromRoute] long tenantId, [FromBody] UpdateProductRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid request model" });
                }

                var productId = await this.Service.UpdateProduct(tenantId, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = "Product updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns>Success message</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpDelete]
        [Route("tenants/{tenantId:long}/{productId:long}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] long tenantId, [FromRoute] long productId)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid product ID" });
                }

                await this.Service.DeleteProduct(tenantId, productId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = "Product deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <returns>List of categories</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpGet]
        [Route("categories")]
        public IActionResult GetAllCategories([FromQuery] long? tenantId = null)
        {
            try
            {
                var categories = this.Service.GetAllCategories(tenantId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = categories });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Category details</param>
        /// <returns>Newly created category</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/add-category")]
        public async Task<IActionResult> AddCategory([FromRoute] long tenantId, [FromBody] AddCategoryRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid request model" });
                }

                var result = await this.Service.AddCategory(tenantId, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Category details</param>
        /// <returns>Success message</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPut]
        [Route("tenants/{tenantId:long}/update-category/{categoryId:long}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] long categoryId, [FromRoute] long tenantId,
            [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (categoryId != request.CategoryId)
                {
                    return BadRequest(new ApiResult { Exception = "Category ID in route does not match request body" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid request model" });
                }

                await this.Service.UpdateCategory(tenantId, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = "Category updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Success message</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpDelete]
        [Route("categories/{categoryId:long}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] long categoryId, [FromQuery] long tenantId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid category ID" });
                }

                if (tenantId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Invalid tenant ID" });
                }

                await this.Service.DeleteCategory(categoryId, tenantId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = "Category deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get menu master with categories
        /// </summary>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <returns>Menu master with associated categories</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [AllowAnonymous]
        [HttpGet]
        [Route("menu/master")]
        public IActionResult GetMenuMaster([FromQuery] long? tenantId = null)
        {
            try
            {
                var menuMaster = this.Service.GetMenuMaster(tenantId);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = menuMaster });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        #endregion

        #region Examples

        [HttpGet]
        [Route("tenants/{tenantId}/category")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(Model.Product.ProductCategory))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Resource not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ApiResult))]
        public IActionResult GetCategory([FromRoute] string tenantId)
        {
            try
            {
                //Local variable
                List<Model.Response.Category> productCategories = new List<Model.Response.Category>();

                //calling service
                productCategories = this.Service.GetCategory(tenantId);

                // Return productMaster
                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = productCategories });
            }
            // key not found exeception
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ApiResult() { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        // [HttpPost]
        // [Route("tenants/{tenantId}/add-category")]
        // [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(Model.Product.ProductCategory))]
        // [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request", typeof(ApiResult))]
        // [SwaggerResponse(StatusCodes.Status404NotFound, "Resource not found", typeof(ApiResult))]
        // [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ApiResult))]
        // public IActionResult AddCategory([FromRoute] string tenantId, [FromBody] Model.Response.CatrtegoryPayload catrtegoryPayload)
        // {
        //     try
        //     {
        //         //calling service
        //         long categoryId = this.Service.AddCategory(Convert.ToInt64(tenantId), catrtegoryPayload);

        //         // Return productMaster
        //         return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = categoryId });
        //     }
        //     // key not found exeception
        //     catch (KeyNotFoundException ex)
        //     {
        //         return StatusCode(StatusCodes.Status404NotFound, new ApiResult() { Exception = ex.Message });
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
        //     }
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productItemList"></param>
        private void MapProductImages(List<ProductItemList> productItemList)
        {
            foreach (var item in productItemList)
            {
                if (item.images != null)
                {
                    item.images = item.images.Select(GetImages).ToList();
                }
            }
        }

        private void MapProductCartImages(List<ProductCartResponse> productItemList)
        {
            foreach (var item in productItemList)
            {
                item.images = item.images.Select(GetImages).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("tenants/{tenantId}/menu-master")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(Model.Product.ProductCategory))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Resource not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ApiResult))]
        public IActionResult GetMenuMaster([FromRoute] string tenantId)
        {
            try
            {
                //calling service
                List < Model.Response.Category > menuMasters = this.Service.GetMenuMaster(tenantId);

                // Return productMaster
                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = menuMasters });
            }
            // key not found exeception
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ApiResult() { Exception = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Remove product from cart
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpDelete]
        [Route("tenants/{tenantId:long}/remove-from-cart")]
        public IActionResult RemoveProductFromCart([FromRoute] long tenantId, [FromBody] RemoveCartPayLoad payload)
        {
            try
            {
                var result = productService.RemoveProductCart(tenantId.ToString(), payload);

                if (result <= 0)
                    return NotFound(new ApiResult { Exception = "Product not found in cart" });

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Remove product from wishlist
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpDelete]
        [Route("tenants/{tenantId:long}/remove-from-wishlist")]
        public IActionResult RemoveProductFromWishList([FromRoute] long tenantId, [FromBody] RemoveWhishListPayload payload)
        {
            try
            {
                if (payload == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request payload is required" });
                }

                if (payload.UserId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                }

                if (payload.ProductId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Product ID is required" });
                }

                if (tenantId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Tenant ID is required" });
                }

                Logger?.LogInformation($"Controller: Removing product {payload.ProductId} from wishlist for user {payload.UserId}, tenant {tenantId}");

                var result = productService.RemoveProductWishList(tenantId.ToString(), payload);

                if (result <= 0)
                {
                    Logger?.LogWarning($"Controller: Product {payload.ProductId} not found in wishlist for user {payload.UserId}");
                    return NotFound(new ApiResult { Exception = "Product not found in wishlist" });
                }

                Logger?.LogInformation($"Controller: Successfully removed product {payload.ProductId} from wishlist. Return value: {result}");

                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = new { removed = true, wishlistItemId = result } });
            }
            catch (ArgumentException ex)
            {
                Logger?.LogWarning($"Controller: Invalid argument: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Controller: Error removing product from wishlist");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get user's wishlist items with product details
        /// </summary>
        /// <param name="tenantId">Tenant ID from route</param>
        /// <param name="request">Request containing userId and other parameters</param>
        /// <returns>Wishlist items with complete product information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/wishlist/items")]
        public async Task<IActionResult> GetWishlistItems([FromRoute] long tenantId, [FromBody] Model.WishList.GetWishlistItemsRequest request)
        {
            try
            {
                Logger.LogInformation($"Controller: GetWishlistItems called - Route tenantId: {tenantId}, Request: {request?.UserId}");

                if (request == null)
                {
                    Logger.LogWarning("Controller: Request body is null");
                    return BadRequest(new ApiResult { Exception = "Request body is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    Logger.LogWarning($"Controller: ModelState validation failed: {string.Join("; ", errors)}");
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                if (request.UserId <= 0)
                {
                    Logger.LogWarning($"Controller: Invalid UserId: {request.UserId}");
                    return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                }

                if (tenantId <= 0)
                {
                    Logger.LogWarning($"Controller: Invalid TenantId: {tenantId}");
                    return BadRequest(new ApiResult { Exception = "Valid Tenant ID is required" });
                }

                // Use tenantId from route (source of truth), but log if request has different value
                if (request.TenantId > 0 && request.TenantId != tenantId)
                {
                    Logger.LogWarning($"Controller: TenantId mismatch - route: {tenantId}, request: {request.TenantId}. Using route tenantId.");
                }

                // Use route tenantId as it's the source of truth
                var wishlistData = await productService.GetUserWishlistItems(request.UserId, tenantId);

                // Generate full URLs for product images in wishlist (same pattern as get-cart)
                if (wishlistData?.Items != null && wishlistData.Items.Any())
                {
                    var httpRequest = HttpContext.Request;
                    var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";
                    
                    foreach (var item in wishlistData.Items)
                    {
                        if (item.Product != null)
                        {
                            // Set image URLs if ImageId is available
                            if (item.Product.ImageId.HasValue && item.Product.ImageId.Value > 0)
                            {
                                item.Product.ImageUrl = $"{baseUrl}/api/1.0/products/{item.Product.ImageId.Value}";
                                item.Product.ThumbnailUrl = $"{baseUrl}/api/1.0/products/{item.Product.ImageId.Value}/thumbnail";
                            }
                            // If ImageId is null but ProductImage (Poster) exists, we can't construct URL without ImageId
                            // The stored procedure should now return ImageId for products with images
                        }
                    }
                }

                Logger.LogInformation($"Controller: GetWishlistItems returned {wishlistData.Items.Count} items for user {request.UserId}");

                return Ok(new ApiResult
                {
                    Data = wishlistData
                });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning($"Controller: User not found: {ex.Message}");
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Controller: Invalid argument: {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Controller: Error getting wishlist items: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Clear entire wishlist for a user
        /// </summary>
        /// <param name="userId">User ID from route</param>
        /// <param name="tenantId">Tenant ID (optional query parameter)</param>
        /// <returns>Wishlist clearing confirmation and statistics</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found or wishlist is empty", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpDelete]
        [Route("wishlist/{userId:long}/clear")]
        public async Task<IActionResult> ClearWishlist([FromRoute] long userId, [FromQuery] long? tenantId = null)
        {
            try
            {
                Logger.LogInformation($"Controller: ClearWishlist called - userId: {userId}, tenantId: {tenantId}");

                if (userId <= 0)
                {
                    Logger.LogWarning($"Controller: Invalid UserId: {userId}");
                    return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                }

                // If tenantId not provided, try to get it from user's wishlist items or use default
                if (!tenantId.HasValue || tenantId.Value <= 0)
                {
                    try
                    {
                        // Try to get tenantId from user's wishlist items (try common tenant IDs)
                        long[] commonTenantIds = { 1, 2, 3 };
                        foreach (var tid in commonTenantIds)
                        {
                            try
                            {
                                var wishlistData = await productService.GetUserWishlistItems(userId, tid);
                                if (wishlistData?.Items != null && wishlistData.Items.Any())
                                {
                                    tenantId = wishlistData.Items.First().TenantId;
                                    Logger.LogInformation($"Controller: Extracted tenantId {tenantId} from user's wishlist items");
                                    break;
                                }
                            }
                            catch
                            {
                                // Continue to next tenant ID
                                continue;
                            }
                        }
                        
                        // If still not found, default to tenant 1
                        if (!tenantId.HasValue || tenantId.Value <= 0)
                        {
                            tenantId = 1;
                            Logger.LogInformation($"Controller: Using default tenantId: {tenantId}");
                        }
                    }
                    catch
                    {
                        // Default to tenant 1 if unable to determine
                        tenantId = 1;
                        Logger.LogInformation($"Controller: Unable to determine tenantId, using default: {tenantId}");
                    }
                }

                var request = new Model.WishList.ClearWishlistRequest
                {
                    UserId = userId,
                    TenantId = tenantId,
                    ClearCompletely = true // Default to hard delete
                };

                // Extract IP address and User-Agent from request headers if not provided
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();

                var result = await this.productService.ClearWishlist(tenantId.Value, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning($"Controller: Clear wishlist failed - {ex.Message}");
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning($"Controller: Invalid argument - {ex.Message}");
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Controller: Error clearing wishlist: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get user's shopping cart with full product details
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Cart request with user details</param>
        /// <returns>Cart items with complete product information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/get-cart")]
        public async Task<IActionResult> GetCart([FromRoute] long tenantId, [FromBody] Model.ProductCart.GetCartRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                // Ensure tenantId in route matches tenantId in request body (if provided)
                if (request.TenantId.HasValue && request.TenantId.Value != tenantId)
                {
                    return BadRequest(new ApiResult { Exception = "Tenant ID in route does not match tenant ID in request body" });
                }

                // Set tenantId from route if not provided in body
                if (!request.TenantId.HasValue)
                {
                    request.TenantId = tenantId;
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                var cartResponse = await this.Service.GetUserCart(request);
                
                // Generate full URLs for product images in cart
                if (cartResponse?.Items != null && cartResponse.Items.Any())
                {
                    var httpRequest = HttpContext.Request;
                    var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";
                    
                    foreach (var item in cartResponse.Items)
                    {
                        if (item.Product?.Images != null && item.Product.Images.Any())
                        {
                            foreach (var image in item.Product.Images)
                            {
                                image.ImageUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}";
                                image.ThumbnailUrl = $"{baseUrl}/api/1.0/products/{image.ImageId}/thumbnail";
                            }
                        }
                    }
                }
                
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = cartResponse });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        /// <param name="request">Add to cart request details</param>
        /// <returns>Cart item details and summary</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]        
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product or user not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Insufficient stock", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenantId/{tenantId:long}/add-cart")]
        public async Task<IActionResult> AddItemToCart([FromRoute] long tenantId, [FromBody] Model.ProductCart.AddToCartRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.AddItemToCart(tenantId,request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Insufficient stock or other business rule violations
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        /// <param name="request">Remove from cart request details</param>
        /// <returns>Removal confirmation and updated cart summary</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found in cart", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenantId/{tenantId:long}/remove-item")]
        public async Task<IActionResult> RemoveItemFromCart([FromRoute] long tenantId, [FromBody] Model.ProductCart.RemoveFromCartRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.RemoveItemFromCart(tenantId, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Clear entire cart
        /// </summary>
        /// <param name="request">Clear cart request details</param>
        /// <returns>Cart clearing confirmation and statistics</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found or cart is empty", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenantId/{tenantId:long}/clear-cart")]
        public async Task<IActionResult> ClearCart([FromRoute] long tenantId, [FromBody] Model.ProductCart.ClearCartRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.ClearCart(tenantId, request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Create a new order from cart items or direct order
        /// </summary>
        /// <param name="request">Create order request details</param>
        /// <returns>Order creation confirmation and details</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Insufficient stock", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("create-orders")]
        public async Task<IActionResult> CreateOrder([FromBody] Model.Order.CreateOrderRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate order items exist
                if (request.Items == null || !request.Items.Any())
                {
                    return BadRequest(new ApiResult { Exception = "Order must contain at least one item" });
                }

                // Validate totals calculation
                var calculatedSubtotal = request.Items.Sum(item => item.Total);
                if (Math.Abs(calculatedSubtotal - request.Totals.Subtotal) > 0.01m)
                {
                    return BadRequest(new ApiResult { Exception = "Subtotal calculation mismatch" });
                }

                var calculatedTotal = request.Totals.Subtotal + request.Totals.Shipping + request.Totals.Tax - request.Totals.Discount;
                if (Math.Abs(calculatedTotal - request.Totals.Total) > 0.01m)
                {
                    return BadRequest(new ApiResult { Exception = "Total calculation mismatch" });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.CreateOrder(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Insufficient stock or other business rule violations
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get user orders with pagination and filtering
        /// </summary>
        /// <param name="request">Get orders request with pagination and filters</param>
        /// <returns>List of orders with pagination information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("get-orders")]
        public async Task<IActionResult> GetOrders([FromBody] Model.Order.GetOrdersRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate pagination parameters
                if (request.Page < 1)
                {
                    request.Page = 1;
                }

                if (request.Limit < 1 || request.Limit > 100)
                {
                    request.Limit = 10;
                }

                var result = await this.Service.GetOrders(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get order details by order ID
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <returns>Detailed order information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpGet]
        [Route("orders/{orderId:long}")]
        public async Task<IActionResult> GetOrderById([FromRoute] long orderId, [FromQuery] long userId, [FromQuery] long? tenantId = null)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Order ID is required" });
                }

                if (userId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                }

                var request = new Model.Order.GetOrderByIdRequest
                {
                    OrderId = orderId,
                    UserId = userId,
                    TenantId = tenantId
                };

                var result = await this.Service.GetOrderById(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Cancel order request details</param>
        /// <returns>Order cancellation confirmation</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Order cannot be cancelled", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPut]
        [Route("orders/{orderId:long}/cancel")]
        public async Task<IActionResult> CancelOrder([FromRoute] long orderId, [FromBody] Model.Order.CancelOrderRequest request = null)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Order ID is required" });
                }

                // If request is null, create a minimal request with just the order ID
                if (request == null)
                {
                    request = new Model.Order.CancelOrderRequest();
                }

                request.OrderId = orderId; // Ensure order ID matches route parameter

                // UserId is required - if not provided in body, it should be extracted from auth context
                // For this example, we'll require it in the request body or query parameter
                if (request.UserId <= 0)
                {
                    // Try to get from query parameter as fallback
                    if (HttpContext.Request.Query.TryGetValue("userId", out var userIdValue) && 
                        long.TryParse(userIdValue, out var userId))
                    {
                        request.UserId = userId;
                    }
                    else
                    {
                        return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.CancelOrder(request);
                
                // Return simple response format as requested
                var simpleResponse = new Model.Order.SimpleOperationResponse
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result
                };

                return StatusCode(StatusCodes.Status200OK, simpleResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Order cannot be cancelled in current status
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Update order status request details</param>
        /// <returns>Order status update confirmation</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Invalid status transition", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("orders/{orderId:long}/status")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] long orderId, [FromBody] Model.Order.UpdateOrderStatusRequest request)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Order ID is required" });
                }

                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                request.OrderId = orderId; // Ensure order ID matches route parameter

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.UpdateOrderStatus(request);
                
                // Return simple response format as requested
                var simpleResponse = new Model.Order.SimpleOperationResponse
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result
                };

                return StatusCode(StatusCodes.Status200OK, simpleResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Invalid status transition
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Bulk update order status (for multiple orders)
        /// </summary>
        /// <param name="request">Bulk update order status request</param>
        /// <returns>Bulk update result</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("orders/update-status")]
        public async Task<IActionResult> BulkUpdateOrderStatus([FromBody] Model.Order.BulkUpdateOrderStatusRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (request.OrderIds == null || request.OrderIds.Count == 0)
                {
                    return BadRequest(new ApiResult { Exception = "At least one Order ID is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.BulkUpdateOrderStatus(request);

                return Ok(new ApiResult { Data = result });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in bulk update order status: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <param name="request">Get all users request with pagination and filters</param>
        /// <returns>List of users with detailed information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("admin/users")]
        public async Task<IActionResult> GetAllUsers([FromBody] Model.Admin.GetAllUsersRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate pagination parameters
                if (request.Page < 1)
                {
                    request.Page = 1;
                }

                if (request.Limit < 1 || request.Limit > 100)
                {
                    request.Limit = 10;
                }

                var result = await this.Service.GetAllUsers(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update user role (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Update user role request details</param>
        /// <returns>Role update confirmation</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("admin/users/{userId:long}/role")]
        public async Task<IActionResult> UpdateUserRole([FromRoute] long userId, [FromBody] Model.Admin.UpdateUserRoleRequest request)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid User ID is required" });
                }

                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                request.UserId = userId; // Ensure user ID matches route parameter

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.UpdateUserRole(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        /// <param name="request">Get all orders request with pagination and filters</param>
        /// <returns>List of orders with detailed information and statistics</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("admin/orders")]
        public async Task<IActionResult> GetAllOrders([FromBody] Model.Admin.GetAllOrdersRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate pagination parameters
                if (request.Page < 1)
                {
                    request.Page = 1;
                }

                if (request.Limit < 1 || request.Limit > 100)
                {
                    request.Limit = 10;
                }

                // Validate date range
                if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate > request.EndDate)
                {
                    return BadRequest(new ApiResult { Exception = "Start date cannot be later than end date" });
                }

                var result = await this.Service.GetAllOrders(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Add multiple images to a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="request">Add product images request with multipart form data</param>
        /// <returns>List of added images</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status413PayloadTooLarge, "File too large", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("tenants/{tenantId:long}/images")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize * 10)] // Allow multiple files
        public async Task<IActionResult> AddProductImages([FromRoute] long tenantId, 
            [FromForm] Model.Product.AddProductImagesRequest request)
        {
            try
            {
                if (request.ProductId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Product ID is required" });
                }

                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate files
                if (request.Images == null || !request.Images.Any())
                {
                    return BadRequest(new ApiResult { Exception = "At least one image file is required" });
                }

                // Validate each file
                foreach (var file in request.Images)
                {
                    if (file == null || file.Length == 0)
                    {
                        return BadRequest(new ApiResult { Exception = "Invalid file uploaded" });
                    }

                    if (file.Length > MaxFileSize)
                    {
                        return BadRequest(new ApiResult { Exception = $"File {file.FileName} exceeds {MaxFileSize / (1024 * 1024)}MB limit" });
                    }

                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    {
                        return BadRequest(new ApiResult { Exception = $"File {file.FileName} has invalid type. Allowed: JPEG, PNG, GIF, WebP" });
                    }
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.AddProductImages(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Update product image properties
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="imageId">Image ID</param>
        /// <param name="request">Update product image request details</param>
        /// <returns>Updated image information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product or image not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPut]
        [Route("products/{productId:long}/images/{imageId:long}")]
        public async Task<IActionResult> UpdateProductImage([FromRoute] long productId, [FromRoute] long imageId, [FromBody] Model.Product.UpdateProductImageRequest request)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Product ID is required" });
                }

                if (imageId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Image ID is required" });
                }

                if (request == null)
                {
                    return BadRequest(new ApiResult { Exception = "Request cannot be null" });
                }

                request.ProductId = productId; // Ensure product ID matches route parameter
                request.ImageId = imageId; // Ensure image ID matches route parameter

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ApiResult { Exception = string.Join("; ", errors) });
                }

                // Validate that at least one property is being updated
                if (!request.Main.HasValue && !request.Active.HasValue && !request.OrderBy.HasValue)
                {
                    return BadRequest(new ApiResult { Exception = "At least one property (main, active, or orderBy) must be specified for update" });
                }

                // Extract IP address and User-Agent from request headers if not provided
                if (string.IsNullOrEmpty(request.IpAddress))
                {
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                if (string.IsNullOrEmpty(request.UserAgent))
                {
                    request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
                }

                var result = await this.Service.UpdateProductImage(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Set main product image
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="imageId">Image ID</param>
        /// <returns>Updated image information</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product or image not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpPost]
        [Route("products/{productId:long}/images/{imageId:long}/set-main")]
        public async Task<IActionResult> SetMainProductImage([FromRoute] long productId, [FromRoute] long imageId)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Product ID is required" });
                }

                if (imageId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Image ID is required" });
                }

                var request = new Model.Product.SetMainProductImageRequest
                {
                    ProductId = productId,
                    ImageId = imageId,
                    // Extract IP address and User-Agent from request headers
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault()
                };

                var result = await this.Service.SetMainProductImage(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }

        /// <summary>
        /// Delete product image
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="imageId">Image ID</param>
        /// <param name="hardDelete">Whether to perform hard delete (permanent) or soft delete</param>
        /// <param name="userId">User ID for activity logging</param>
        /// <returns>Deletion confirmation and remaining images</returns>
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product or image not found", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Cannot delete last main image", typeof(ApiResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiResult))]
        [HttpDelete]
        [Route("products/{productId:long}/images/{imageId:long}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] long productId, [FromRoute] long imageId, [FromQuery] bool hardDelete = false, [FromQuery] long? userId = null)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Product ID is required" });
                }

                if (imageId <= 0)
                {
                    return BadRequest(new ApiResult { Exception = "Valid Image ID is required" });
                }

                var request = new Model.Product.DeleteProductImageRequest
                {
                    ProductId = productId,
                    ImageId = imageId,
                    UserId = userId,
                    HardDelete = hardDelete
                };

                // Extract IP address and User-Agent from request headers
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();

                var result = await this.Service.DeleteProductImage(request);
                return StatusCode(StatusCodes.Status200OK, new ApiResult { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResult { Exception = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Cannot delete last main image
                return StatusCode(StatusCodes.Status409Conflict, new ApiResult { Exception = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResult { Exception = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { Exception = ex.Message });
            }
        }
        #endregion
    }
}
