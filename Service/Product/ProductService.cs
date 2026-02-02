using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Tenant.API.Base.Model.Validation;
using Tenant.API.Base.Repository;
using Tenant.API.Base.Service;
using Tenant.Query.Model.Product;
using Tenant.Query.Model.ProductCart;
using Tenant.Query.Model.Response;
using Tenant.Query.Model.Response.ProductMaster;
using Tenant.Query.Model.WishList;
using Tenant.Query.Model.Email;
using Tenant.Query.Repository.User;
using Tenant.Query.Service.Email;
using Tenant.Query.Model.Settings;
using UnitsNet;

namespace Tenant.Query.Service.Product
{
    public class ProductService : TnBaseService
    {
        #region Private property

        private Repository.Product.ProductRepository productRepository;
        private readonly UserRepository userRepository;
        private readonly EmailService emailService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;

        #endregion

        public ProductService(Repository.Product.ProductRepository productRepository,
                            UserRepository userRepository,
                            EmailService emailService,
                            IConfiguration configuration,
                            ILoggerFactory loggerFactory,
                            TnAudit xcAudit,
                            TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            this.productRepository = productRepository;
            this.userRepository = userRepository;
            this.emailService = emailService;
            this._loggerFactory = loggerFactory;
            this._configuration = configuration;
            this.productRepository.Logger = loggerFactory.CreateLogger<Repository.Product.ProductRepository>();
        }


        #region Get End Point
        public List<Model.Response.Category> GetMenuMaster(string tenantId)
        {
            try
            {
                string spName = Model.Constant.Constant.StoredProcedures.HN_GET_MENU_MASTER;

                var productCategories = this.productRepository.GetMenuMaster(tenantId, spName) ?? new List<Model.Product.ProductCategory>();

                return productCategories
                    .Where(x => x?.SubCategory == 0)
                    .OrderBy(x => x.OrderBy)
                    .Select(category => new Model.Response.Category
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category,
                        Order = category.OrderBy,
                        subMenu = category.SubMenu,
                        link = category.Link,
                        subCategories = productCategories
                            .Where(sub => sub?.SubCategory == category.CategoryId)
                            .Select(sub => new Model.Response.SubCategory
                            {
                                Id = sub.CategoryId,
                                Name = sub.Category,
                                Order = sub.OrderBy
                            })
                            .ToList()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Category> GetCategories(string tenantId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public async Task<byte[]> CreateThumbnailAsync(byte[] imageData, int width, int height)
        {
            using var imageStream = new MemoryStream(imageData);
            using var image = await Image.LoadAsync(imageStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max
            }));

            using var resultStream = new MemoryStream();
            await image.SaveAsync(resultStream, new JpegEncoder
            {
                Quality = 80
            });
            return resultStream.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public bool IsImageValid(byte[] imageData)
        {
            try
            {
                if (imageData == null || imageData.Length == 0)
                {
                    this._loggerFactory?.CreateLogger<ProductService>()?.LogWarning("Image validation failed: Image data is null or empty");
                    return false;
                }

                // Try multiple approaches to validate the image
                // Approach 1: Use Image.Identify (lightweight, doesn't decode full image)
                try
                {
                    using var imageStream1 = new MemoryStream(imageData, false);
                    imageStream1.Position = 0;
                    
                    var imageInfo = Image.Identify(imageStream1);
                    
                    if (imageInfo != null && imageInfo.Width > 0 && imageInfo.Height > 0)
                    {
                        this._loggerFactory?.CreateLogger<ProductService>()?.LogInformation($"Image validation successful (Identify). Format: {imageInfo.Metadata?.DecodedImageFormat?.Name ?? "Unknown"}, Dimensions: {imageInfo.Width}x{imageInfo.Height}, Size: {imageData.Length} bytes");
                        return true;
                    }
                }
                catch (Exception identifyEx)
                {
                    this._loggerFactory?.CreateLogger<ProductService>()?.LogWarning($"Image.Identify failed: {identifyEx.Message}. Trying Image.Load as fallback...");
                }

                // Approach 2: Try to actually load the image (more thorough validation)
                try
                {
                    using var imageStream2 = new MemoryStream(imageData, false);
                    imageStream2.Position = 0;
                    
                    using var image = Image.Load(imageStream2);
                    
                    if (image != null && image.Width > 0 && image.Height > 0)
                    {
                        this._loggerFactory?.CreateLogger<ProductService>()?.LogInformation($"Image validation successful (Load). Format: {image.Metadata?.DecodedImageFormat?.Name ?? "Unknown"}, Dimensions: {image.Width}x{image.Height}, Size: {imageData.Length} bytes");
                        return true;
                    }
                }
                catch (Exception loadEx)
                {
                    this._loggerFactory?.CreateLogger<ProductService>()?.LogWarning($"Image.Load failed: {loadEx.Message}");
                }

                // If both approaches failed, log the first few bytes for debugging
                string hexPreview = imageData.Length >= 16 
                    ? string.Join(" ", imageData.Take(16).Select(b => b.ToString("X2")))
                    : string.Join(" ", imageData.Select(b => b.ToString("X2")));
                
                this._loggerFactory?.CreateLogger<ProductService>()?.LogWarning($"Image validation failed: Both Identify and Load failed. File size: {imageData.Length} bytes, First 16 bytes (hex): {hexPreview}");
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception if logger is available
                this._loggerFactory?.CreateLogger<ProductService>()?.LogError(ex, $"Image validation failed with exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Model.Product.ProductImages> GetImageAsync(long Id)
        {
            var productImage = new Model.Product.ProductImages();
            DataTable productImageDt = await Task.Run(() => productRepository.GetImageAsync(Id));
            if (productImageDt?.Rows.Count > 0)
            {
                MapProductImage(productImage, productImageDt.Rows[0]);
            }

            return productImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productImage"></param>
        /// <param name="row"></param>
        private void MapProductImage(Model.Product.ProductImages productImage, DataRow row)
        {
            productImage.ImageData = GetColumnValue<byte[]>(row, "ImageData", null);
            productImage.ContentType = GetColumnValue<string>(row, "ContentType", string.Empty);
            productImage.ImageName = GetColumnValue<string>(row, "ImageName", string.Empty);
            productImage.ThumbnailData = GetColumnValue<byte[]>(row, "ThumbnailData", null);
            productImage.ProductId = GetColumnValue<long>(row, "ProductId", 0);
            productImage.Active = GetColumnValue<bool>(row, "", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public long AddImages(ProductNewImage images)
        {
            try
            {
                return productRepository.AddImages(images.ProductId, images.ImageName, images.ContentType, images.ImageData, images.ThumbnailData, images.FileSize);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// DeleteImages
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long DeleteImage(long id)
        {
            try
            {
                return productRepository.DeleteImages(id);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new Exception("An error occurred while deleting the image.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public long AddProductCategory(long tenantId, Model.Product.ProductCategoryPayload productCategory)
        {
            try
            {
                long productCategoryId = this.productRepository.AddProductCategory(tenantId, productCategory);
                return productCategoryId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="tenantId"></param>
        // /// <param name="productCategory"></param>
        // /// <returns></returns>
        // public long AddCategory(long tenantId, Model.Response.CatrtegoryPayload catrtegoryPayload)
        // {
        //     try
        //     {
        //         long categoryId = this.productRepository.AddCategory(tenantId, catrtegoryPayload);
        //         return categoryId;
        //     }
        //     catch (Exception)
        //     {
        //         throw;
        //     }
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<ProductCartResponse> UpsertCart(long tenantId, CartPayload payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload), "Payload cannot be null.");

            if (tenantId <= 0)
                throw new ArgumentException("Invalid tenant ID.", nameof(tenantId));

            try
            {
                var productCartResponse = new ProductCartResponse();
                var dtProductCartResponse = await Task.Run(() => productRepository.UpsertCart(payload));

                if (dtProductCartResponse == null || dtProductCartResponse.Rows.Count == 0)
                    return new ProductCartResponse();

                var productPayload = CreateProductPayload(payload.ProductId);
                var spName = Model.Constant.Constant.XC_GET_PRODUCT_MASTER_LIST_TESTING;

                var (orderBy, order) = ParseOrderBy(productPayload.OrderBy);
                var response = GetProductList(spName, tenantId.ToString(), productPayload, orderBy, order);

                if (response == null || !response.Any())
                    return new ProductCartResponse();

                return MapProductCartResponse(payload, dtProductCartResponse, response.First());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentException("Invalid argument provided.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while upserting the cart.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<ProductWishListResponse> UpsertWishList(long tenantId, WishListPayload payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload), "Payload cannot be null.");

            if (tenantId <= 0)
                throw new ArgumentException("Invalid tenant ID.", nameof(tenantId));

            try
            {
                var dtProductWishList = await Task.Run(() => productRepository.UpsertWishList(payload));

                if (dtProductWishList == null || dtProductWishList.Rows.Count == 0)
                    return new ProductWishListResponse();

                // SP_UPSERT_WISHLIST now returns product details directly, so we can map from DataTable
                var row = dtProductWishList.Rows[0];
                
                // Get product images - safely check if columns exist before accessing
                var images = new List<Model.Product.ImageResponseDto>();
                var imageId = GetColumnValue<long?>(row, "ImageId");
                if (imageId.HasValue && imageId.Value > 0)
                {
                    var imageUrl = GetColumnValue<string>(row, "ImageUrl", "");
                    images.Add(new Model.Product.ImageResponseDto
                    {
                        Id = imageId.Value,
                        ImageUrl = imageUrl,
                        ThumbnailUrl = imageUrl, // Use same URL for thumbnail
                        ContentType = "image/jpeg", // Default, can be enhanced if stored
                        ImageName = ""
                    });
                }

                var quantity = GetColumnValue<long>(row, "Quantity", 1);
                var price = GetColumnValue<decimal>(row, "Price", 0);

                return new ProductWishListResponse
                {
                    UserId = payload.UserId,
                    ProductId = payload.ProductId,
                    Quantity = quantity,
                    Price = price,
                    Total = price * quantity,
                    ProductName = GetColumnValue<string>(row, "ProductName", ""),
                    images = images
                };
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentException("Invalid argument provided.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while upserting the cart.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public List<ProductCartResponse> GetUserCart(long userId)
        {
            if (userId == 0)
                throw new ArgumentNullException(nameof(userId), "Payload cannot be null.");

            try
            {
                var productWishListResponse = new List<ProductCartResponse>();
                List<Model.ProductCart.SpProductCart> spProductCarts = productRepository.GetUserCart("SP_CUSTOMER_CART", userId);

                if (spProductCarts == null || spProductCarts.Count == 0)
                    return new List<ProductCartResponse>();

                var productPayload = CreateProductListPayload(spProductCarts.Select(x => x.ProductId).ToList());
                var spName = Model.Constant.Constant.XC_GET_PRODUCT_MASTER_LIST_TESTING;

                var (orderBy, order) = ParseOrderBy(productPayload.OrderBy);
                var response = GetProductList(spName, "10", productPayload, orderBy, order);

                if (response == null || !response.Any())
                    return new List<ProductCartResponse>();

                // Map the response to a list of ProductWishListResponse
                foreach (var product in response)
                {
                    var wishListResponse = new ProductCartResponse
                    {

                        UserId = spProductCarts.FirstOrDefault()?.UserId ?? 0,
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Quantity = spProductCarts.FirstOrDefault(x => x.ProductId == product.ProductId)?.Quantity ?? 0,
                        images = product.images ?? new List<Model.Product.ImageResponseDto>()
                    };
                    productWishListResponse.Add(wishListResponse);
                }

                return productWishListResponse;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentException("Invalid argument provided.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user cart.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private Model.Product.ProductPayload CreateProductPayload(long productId)
        {
            return new Model.Product.ProductPayload
            {
                Page = 1,
                PageSize = 10,
                Category = 1,
                OrderBy = "ProductName,asc",
                ProductId = new List<long> { productId },
                RoleId = "1",
                Search = ""
            };
        }

        private Model.Product.ProductPayload CreateProductListPayload(List<long> productIds)
        {
            return new Model.Product.ProductPayload
            {
                Page = 1,
                PageSize = 10,
                Category = 0,
                OrderBy = "ProductName,asc",
                ProductId = productIds,
                RoleId = "1",
                Search = ""
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderByClause"></param>
        /// <returns></returns>
        private (string orderBy, string order) ParseOrderBy(string orderByClause)
        {
            var filters = orderByClause?.Split(',');
            var orderBy = string.IsNullOrEmpty(filters?.ElementAtOrDefault(0)) ? Model.Constant.Constant.PrductName : filters[0];
            var order = string.IsNullOrEmpty(filters?.ElementAtOrDefault(1)) ? Model.Constant.Constant.ASC : filters[1];
            return (orderBy, order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="dtProductCartResponse"></param>
        /// <param name="firstProduct"></param>
        /// <returns></returns>
        private ProductCartResponse MapProductCartResponse(CartPayload payload, DataTable dtProductCartResponse, Model.Product.ProductItemList firstProduct)
        {
            return new ProductCartResponse
            {
                UserId = payload.UserId,
                ProductId = payload.ProductId,
                Quantity = Convert.ToInt64(dtProductCartResponse.Rows[0]["QUANTITY"]),
                Price = firstProduct.Price,
                Total = firstProduct.Price * Convert.ToInt64(dtProductCartResponse.Rows[0]["QUANTITY"]),
                ProductName = firstProduct.ProductName,
                images = firstProduct.images ?? new List<Model.Product.ImageResponseDto>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="dtProductCartResponse"></param>
        /// <param name="firstProduct"></param>
        /// <returns></returns>
        private ProductWishListResponse MapProductWishList(WishListPayload payload, DataTable dtProductWishList, Model.Product.ProductItemList firstProduct)
        {
            return new ProductWishListResponse
            {
                UserId = payload.UserId,
                ProductId = payload.ProductId,
                Quantity = Convert.ToInt64(dtProductWishList.Rows[0]["QUANTITY"]),
                Price = firstProduct.Price,
                Total = firstProduct.Price * Convert.ToInt64(dtProductWishList.Rows[0]["QUANTITY"]),
                ProductName = firstProduct.ProductName,
                images = firstProduct.images ?? new List<Model.Product.ImageResponseDto>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetColumnValue<T>(DataRow row, string columnName, T defaultValue = default)
        {
            try
            {
                if (!row.Table.Columns.Contains(columnName))
                {
                    return defaultValue;
                }

                var value = row[columnName];
                
                if (value == null || value == DBNull.Value)
                {
                    return defaultValue;
                }

                // Handle nullable types
                var underlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (underlyingType != null)
                {
                    // It's a nullable type, convert to underlying type
                    if (value == DBNull.Value || value == null)
                    {
                        return defaultValue;
                    }
                    var convertedValue = Convert.ChangeType(value, underlyingType);
                    return (T)convertedValue;
                }

                // Non-nullable type
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                // If conversion fails, return default value
                return defaultValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<Model.Response.Category> GetCategory(string tenantId)
        {
            try
            {
                List<Model.Product.ProductCategory> productCategories = this.productRepository.GetCategory(tenantId);

                List<Model.Response.Category> categories = new List<Model.Response.Category>();

                productCategories.Where(x => x.SubCategory == 0).OrderBy(x => x.CategoryId).ToList().ForEach(category =>
                {
                    Model.Response.Category cate = new Model.Response.Category();
                    List<Model.Response.SubCategory> categoryList = new List<Model.Response.SubCategory>();

                    List<Model.Product.ProductCategory> subCategories = productCategories.Where(x => x.SubCategory == category.CategoryId).ToList();

                    cate.CategoryId = category.CategoryId;
                    cate.Name = category.Category;

                    subCategories.ForEach(sub =>
                    {
                        Model.Response.SubCategory subCategory = new Model.Response.SubCategory();
                        subCategory.Id = sub.CategoryId;
                        subCategory.Name = sub.Category;
                        subCategory.Order = sub.CategoryId;
                        categoryList.Add(subCategory);
                    });
                    cate.subCategories = categoryList;

                    categories.Add(cate);
                });

                //string psw = EnDecrypt("I0FEO09BK0VEEN9DGJ9FGU8BMQ8DKTSP", false);

                return categories;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public List<Model.Product.ProductItemList> GetProductList(string tenantId, Model.Product.ProductPayload payload)
        {
            try
            {
                string spName = Model.Constant.Constant.XC_GET_PRODUCT_MASTER_LIST_TESTING;
                string[] filters = payload.OrderBy?.Split(',');
                string orderBy = string.IsNullOrEmpty(filters?.ElementAtOrDefault(0)) ? Model.Constant.Constant.PrductName : filters[0];
                string order = string.IsNullOrEmpty(filters?.ElementAtOrDefault(1)) ? Model.Constant.Constant.ASC : filters[1];


                List<Model.Product.ProductItemList> response = this.GetProductList(spName, tenantId, payload, orderBy, order);

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// GetProductList
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <param name="orderBy"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private List<ProductItemList> GetProductList(string spName, string tenantId, Model.Product.ProductPayload payload, string orderBy, string order)
        {
            List<Model.Product.SpProductMasterList> SpProductMasterList = this.productRepository.GetProductList(spName,
                    tenantId,
                    payload,
                    orderBy,
                    order);

            List<Model.Product.ProductItemList> productItemList = new List<Model.Product.ProductItemList>();
            if (SpProductMasterList == null || SpProductMasterList.Count < 1)
                return productItemList;

            Model.Product.ProductItemList mapitem;
            foreach (var productItems in SpProductMasterList.GroupBy(p => p.ProductId).Select(g => g.First()))
            {
                mapitem = new Model.Product.ProductItemList()
                {
                    ProductId = productItems.ProductId,
                    ProductName = productItems.ProductName,
                    Description = productItems.Description,
                    Price = productItems.Price,
                    Rating = productItems.Rating,
                    Stock = productItems.Stock,
                    BestSeller = productItems.BestSeller,
                    TenantId = productItems.TenantId,
                    Quantity = productItems.Quantity,
                    Numofreviews = productItems.Numofreviews,
                    Displayname = productItems.Displayname,
                    Guid = productItems.Guid,
                    Created = productItems.Created,
                    LastModified = productItems.LastModified,
                    LastModifiedBy = productItems.LastModifiedBy,
                    images = SpProductMasterList.Where(p => p.ProductId == productItems.ProductId)
                    .Select(image => new Model.Product.ImageResponseDto
                    {
                        Id = image.Id,
                        ImageUrl = $"/Products/GetImage/{image.Id}",
                        ThumbnailUrl = $"/Products/{image.Id}/GetThumbnail",
                        ContentType = image.ContentType,
                        ImageName = image.ImageName,
                    })
                    .ToList()
                };
                productItemList.Add(mapitem);
            }

            return productItemList;
        }

        public static string EnDecrypt(string input, bool decrypt = false)
        {
            string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ984023";

            if (decrypt)
            {
                Dictionary<string, uint> _index = null;
                Dictionary<string, Dictionary<string, uint>> _indexes =
                    new Dictionary<string, Dictionary<string, uint>>(2, StringComparer.InvariantCulture);

                if (_index == null)
                {
                    Dictionary<string, uint> cidx;

                    string indexKey = "I" + _alphabet;

                    if (!_indexes.TryGetValue(indexKey, out cidx))
                    {
                        lock (_indexes)
                        {
                            if (!_indexes.TryGetValue(indexKey, out cidx))
                            {
                                cidx = new Dictionary<string, uint>(_alphabet.Length, StringComparer.InvariantCulture);
                                for (int i = 0; i < _alphabet.Length; i++)
                                {
                                    cidx[_alphabet.Substring(i, 1)] = (uint)i;
                                }

                                _indexes.Add(indexKey, cidx);
                            }
                        }
                    }

                    _index = cidx;
                }

                MemoryStream ms = new MemoryStream(Math.Max((int)Math.Ceiling(input.Length * 5 / 8.0), 1));

                for (int i = 0; i < input.Length; i += 8)
                {
                    int chars = Math.Min(input.Length - i, 8);

                    ulong val = 0;

                    int bytes = (int)Math.Floor(chars * (5 / 8.0));

                    for (int charOffset = 0; charOffset < chars; charOffset++)
                    {
                        uint cbyte;
                        if (!_index.TryGetValue(input.Substring(i + charOffset, 1), out cbyte))
                        {
                            throw new ArgumentException(string.Format("Invalid character {0} valid characters are: {1}",
                                input.Substring(i + charOffset, 1), _alphabet));
                        }

                        val |= (((ulong)cbyte) << ((((bytes + 1) * 8) - (charOffset * 5)) - 5));
                    }

                    byte[] buff = BitConverter.GetBytes(val);
                    Array.Reverse(buff);
                    ms.Write(buff, buff.Length - (bytes + 1), bytes);
                }

                return System.Text.ASCIIEncoding.ASCII.GetString(ms.ToArray());
            }
            else
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(input);

                StringBuilder result = new StringBuilder(Math.Max((int)Math.Ceiling(data.Length * 8 / 5.0), 1));

                byte[] emptyBuff = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                byte[] buff = new byte[8];

                for (int i = 0; i < data.Length; i += 5)
                {
                    int bytes = Math.Min(data.Length - i, 5);

                    Array.Copy(emptyBuff, buff, emptyBuff.Length);
                    Array.Copy(data, i, buff, buff.Length - (bytes + 1), bytes);
                    Array.Reverse(buff);
                    ulong val = BitConverter.ToUInt64(buff, 0);

                    for (int bitOffset = ((bytes + 1) * 8) - 5; bitOffset > 3; bitOffset -= 5)
                    {
                        result.Append(_alphabet[(int)((val >> bitOffset) & 0x1f)]);
                    }
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Removes a product from the cart.
        /// </summary>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="payload">The payload containing product and user details.</param>
        /// <returns>The result of the removal operation.</returns>
        public long RemoveProductCart(string tenantId, RemoveCartPayLoad payload)
        {
            try
            {
                return productRepository.RemoveProductCart(tenantId, payload);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new Exception("An error occurred while removing the product from the cart.", ex);
            }
        }

        /// <summary>
        /// Removes a product from the wishlist.
        /// </summary>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="payload">The payload containing product and user details.</param>
        /// <returns>The result of the removal operation.</returns>
        public long RemoveProductWishList(string tenantId, RemoveWhishListPayload payload)
        {
            try
            {
                return productRepository.RemoveProductWishList(tenantId, payload);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new Exception("An error occurred while removing the product from the wishlist.", ex);
            }
        }  

        public string GetValueByKey(string key)
        {
            try
            {
                return productRepository.GetConfigValueByKey(key);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new Exception("An error occurred while retrieving the value by key.", ex);
            }  
        }

        public GetAppSettingsResponse GetAppSettings(long? tenantId, long? userId)
        {
            try
            {
                var settings = productRepository.GetAppSettings(tenantId, userId);
                return new GetAppSettingsResponse
                {
                    Settings = settings
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving settings.", ex);
            }
        }

        public SaveAppSettingsResponse SaveAppSettings(SaveAppSettingsRequest request)
        {
            try
            {
                if (request == null || request.Settings == null || request.Settings.Count == 0)
                {
                    return new SaveAppSettingsResponse
                    {
                        Success = false,
                        Message = "Settings payload is empty."
                    };
                }

                foreach (var setting in request.Settings)
                {
                    if (setting == null || string.IsNullOrWhiteSpace(setting.SettingKey))
                    {
                        continue;
                    }

                    productRepository.UpsertAppSetting(setting, request.TenantId, request.UserId);
                }

                return new SaveAppSettingsResponse();
            }
            catch (Exception ex)
            {
                return new SaveAppSettingsResponse
                {
                    Success = false,
                    Message = $"Error saving settings: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns>Task</returns>
        public async Task DeleteProduct(long tenantId, long productId)
        {
            try
            {
                if (tenantId <= 0)
                    throw new ArgumentException("Invalid tenant ID", nameof(tenantId));

                if (productId <= 0)
                    throw new ArgumentException("Invalid product ID", nameof(productId));

                // Get current user ID from context or pass it as parameter
                long userId = 1; // TODO: Get from context

                // Call repository to delete product
                await productRepository.DeleteProduct(tenantId, productId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <returns>List of categories</returns>
        public List<CategoryListItem> GetAllCategories(long? tenantId = null)
        {
            try
            {
                return productRepository.GetAllCategories(tenantId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving categories.", ex);
            }
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Category details</param>
        /// <returns>Newly created category</returns>
        public async Task<AddCategoryResponse> AddCategory(long tenantId, AddCategoryRequest request)
        {
            try
            {
                if (tenantId <= 0)
                    throw new ArgumentException("Invalid tenant ID", nameof(tenantId));

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrWhiteSpace(request.CategoryName))
                    throw new ArgumentException("Category name is required");

                // Get current user ID from context or pass it as parameter
                long userId = 1; // TODO: Get from context

                // Call repository to add category
                var categoryId = await productRepository.AddCategory(tenantId, request, userId);

                // Return the newly created category
                return new AddCategoryResponse
                {
                    CategoryId = categoryId,
                    Category = request.CategoryName,
                    Active = request.Active,
                    ParentId = request.ParentCategoryId,
                    Description = request.Description,
                    OrderBy = request.OrderBy,
                    Icon = request.Icon,
                    SubMenu = request.HasSubMenu,
                    Link = request.Link,
                    Created = DateTime.UtcNow,
                    TenantId = tenantId
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Category details</param>
        /// <returns>Task</returns>
        public async Task UpdateCategory(long tenantId, UpdateCategoryRequest request)
        {
            try
            {
                if (tenantId <= 0)
                    throw new ArgumentException("Invalid tenant ID", nameof(tenantId));

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.CategoryId <= 0)
                    throw new ArgumentException("Invalid category ID", nameof(request.CategoryId));

                if (string.IsNullOrWhiteSpace(request.CategoryName))
                    throw new ArgumentException("Category name is required");

                // Get current user ID from context or pass it as parameter
                long userId = 1; // TODO: Get from context

                // Call repository to update category
                await productRepository.UpdateCategory(tenantId, request, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        /// <summary>
        /// Get menu master with categories
        /// </summary>
        /// <param name="tenantId">Optional tenant ID filter</param>
        /// <returns>Menu master with associated categories</returns>
        public MenuMasterResponse GetMenuMaster(long? tenantId = null)
        {
            try
            {
                return productRepository.GetMenuMaster(tenantId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving menu master.", ex);
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Product details</param>
        /// <returns>Product ID</returns>
        public async Task<long> UpdateProduct(long tenantId, UpdateProductRequest request)
        {
            try
            {
                if (tenantId <= 0)
                    throw new ArgumentException("Invalid tenant ID", nameof(tenantId));

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.ProductId <= 0)
                    throw new ArgumentException("Invalid product ID", nameof(request.ProductId));

                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.ProductName))
                    throw new ArgumentException("Product name is required");

                if (string.IsNullOrWhiteSpace(request.ProductCode))
                    throw new ArgumentException("Product code is required");

                if (request.Price <= 0)
                    throw new ArgumentException("Price must be greater than zero");

                if (request.Category <= 0)
                    throw new ArgumentException("Category is required");

                // Call repository to update product
                return await productRepository.UpdateProduct(tenantId, request);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Product details</param>
        /// <returns>Product ID</returns>
        public async Task<long> AddProduct(long tenantId, AddProductRequest request)
        {
            try
            {
                if (tenantId <= 0)
                    throw new ArgumentException("Invalid tenant ID", nameof(tenantId));

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.ProductName))
                    throw new ArgumentException("Product name is required");

                if (string.IsNullOrWhiteSpace(request.ProductCode))
                    throw new ArgumentException("Product code is required");

                if (request.Price <= 0)
                    throw new ArgumentException("Price must be greater than zero");

                if (request.Category <= 0)
                    throw new ArgumentException("Category is required");

                // Call repository to add product
                return await productRepository.AddProduct(tenantId, request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get product details by ID
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Product details with images</returns>
        public async Task<ProductDetailItem> GetProductById(long productId)
        {
            try
            {
                if (productId <= 0)
                    throw new ArgumentException("Invalid product ID", nameof(productId));

                var result = await productRepository.GetProductById(productId);

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    return null;

                // Map product details
                var product = MapProductSearchResults(result.Tables[0]).FirstOrDefault();

                if (product != null && result.Tables.Count > 1)
                {
                    // Map images
                    product.Images = MapProductImages(result.Tables[1], productId);
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the product.", ex);
            }
        }

        /// <summary>
        /// Load images for multiple products in batch
        /// </summary>
        /// <param name="productIds">List of product IDs</param>
        /// <returns>Dictionary mapping product ID to list of images</returns>
        private async Task<Dictionary<long, List<ProductSearchImageInfo>>> LoadProductImagesBatchAsync(List<long> productIds)
        {
            var result = new Dictionary<long, List<ProductSearchImageInfo>>();
            
            if (productIds == null || !productIds.Any())
                return result;
            
            // Initialize all products with empty image lists
            foreach (var productId in productIds)
            {
                result[productId] = new List<ProductSearchImageInfo>();
            }
            
            // Load images for each product (in parallel for better performance)
            var tasks = productIds.Select(async productId =>
            {
                try
                {
                    var productResult = await productRepository.GetProductById(productId);
                    
                    if (productResult != null && productResult.Tables.Count > 1 && productResult.Tables[1].Rows.Count > 0)
                    {
                        var images = MapProductImages(productResult.Tables[1], productId);
                        // URLs will be generated in the controller using Url.ActionLink
                        return new { ProductId = productId, Images = images };
                    }
                }
                catch (Exception ex)
                {
                    this._loggerFactory?.CreateLogger<ProductService>()?.LogWarning($"Error loading images for product {productId}: {ex.Message}");
                }
                
                return new { ProductId = productId, Images = new List<ProductSearchImageInfo>() };
            });
            
            // Wait for all tasks and populate result dictionary
            var results = await Task.WhenAll(tasks);
            foreach (var item in results)
            {
                result[item.ProductId] = item.Images;
            }
            
            return result;
        }

        /// <summary>
        /// Maps DataTable to list of product images
        /// </summary>
        private List<ProductSearchImageInfo> MapProductImages(DataTable dataTable, long productId = 0)
        {
            var images = new List<ProductSearchImageInfo>();

            foreach (DataRow row in dataTable.Rows)
            {
                var imageId = GetColumnValue<long>(row, "ImageId");
                images.Add(new ProductSearchImageInfo
                {
                    ImageId = imageId,
                    ProductId = productId > 0 ? productId : GetColumnValue<long>(row, "ProductId", 0),
                    Poster = GetColumnValue<string>(row, "Poster", string.Empty),
                    ImageUrl = string.Empty, // Will be set in controller with full URL
                    ThumbnailUrl = string.Empty, // Will be set in controller with full URL
                    Main = GetColumnValue<bool>(row, "Main"),
                    Active = GetColumnValue<bool>(row, "Active"),
                    OrderBy = GetColumnValue<int>(row, "OrderBy"),
                    Created = GetColumnValue<DateTime>(row, "Created", DateTime.UtcNow),
                    Modified = GetColumnValue<DateTime>(row, "Modified", DateTime.UtcNow)
                });
            }

            return images;
        }

        /// <summary>
        /// Search products with advanced filtering and pagination (async version with images)
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="payload">Search parameters</param>
        /// <returns>Product search results with pagination and images</returns>
        public async Task<ProductSearchResponse> SearchProductsAsync(string tenantId, ProductSearchPayload payload)
        {
            try
            {
                if (payload == null)
                    throw new ArgumentNullException(nameof(payload), "Search payload cannot be null.");

                // Calculate offset for pagination
                int offset = (payload.Page - 1) * payload.Limit;

                // Call repository method to get search results
                var searchResults = await Task.Run(() => productRepository.SearchProducts(tenantId, payload, offset));

                // Map the results to response model
                var response = new ProductSearchResponse();

                if (searchResults != null && searchResults.Tables.Count >= 2)
                {
                    // First table contains product data
                    var productTable = searchResults.Tables[0];
                    // Second table contains total count
                    var countTable = searchResults.Tables[1];

                    // Map products
                    response.Products = MapProductSearchResults(productTable);

                    // Load images for all products in batch
                    var productIds = response.Products.Select(p => p.ProductId).ToList();
                    var allImages = await LoadProductImagesBatchAsync(productIds);
                    
                    // Assign images to each product
                    foreach (var product in response.Products)
                    {
                        product.Images = allImages.ContainsKey(product.ProductId) 
                            ? allImages[product.ProductId] 
                            : new List<ProductSearchImageInfo>();
                    }

                    // Map pagination info
                    int totalCount = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;
                    response.Pagination = new PaginationInfo
                    {
                        Page = payload.Page,
                        Limit = payload.Limit,
                        Total = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / payload.Limit),
                        HasNext = payload.Page * payload.Limit < totalCount,
                        HasPrevious = payload.Page > 1
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while searching products.", ex);
            }
        }

        /// <summary>
        /// Search products with advanced filtering and pagination (synchronous wrapper for backward compatibility)
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="payload">Search parameters</param>
        /// <returns>Product search results with pagination</returns>
        public ProductSearchResponse SearchProducts(string tenantId, ProductSearchPayload payload)
        {
            return SearchProductsAsync(tenantId, payload).Result;
        }

        /// <summary>
        /// Enhanced product search with full-text, fuzzy matching, and advanced filters
        /// </summary>
        public async Task<ProductSearchResponse> SearchProductsEnhancedAsync(string tenantId, EnhancedProductSearchPayload payload)
        {
            try
            {
                if (payload == null)
                    throw new ArgumentNullException(nameof(payload), "Search payload cannot be null.");

                // Calculate offset for pagination
                int offset = (payload.Page - 1) * payload.Limit;

                // Call repository method to get enhanced search results
                var searchResults = await Task.Run(() => productRepository.SearchProductsEnhanced(tenantId, payload, offset));

                // Map the results to response model
                var response = new ProductSearchResponse();

                if (searchResults != null && searchResults.Tables.Count >= 2)
                {
                    // First table contains product data with relevance scores
                    var productTable = searchResults.Tables[0];
                    // Second table contains pagination metadata
                    var paginationTable = searchResults.Tables[1];

                    // Map products (enhanced version includes RelevanceScore, NameMatchPos, DescMatchPos)
                    response.Products = MapProductSearchResults(productTable);

                    // Load images for all products in batch
                    var productIds = response.Products.Select(p => p.ProductId).ToList();
                    var allImages = await LoadProductImagesBatchAsync(productIds);
                    
                    foreach (var product in response.Products)
                    {
                        product.Images = allImages.ContainsKey(product.ProductId) 
                            ? allImages[product.ProductId] 
                            : new List<ProductSearchImageInfo>();
                    }

                    // Map pagination info from second result set
                    if (paginationTable.Rows.Count > 0)
                    {
                        var row = paginationTable.Rows[0];
                        response.Pagination = new PaginationInfo
                        {
                            Page = Convert.ToInt32(row["CurrentPage"]),
                            Limit = Convert.ToInt32(row["PageSize"]),
                            Total = Convert.ToInt32(row["TotalCount"]),
                            TotalPages = Convert.ToInt32(row["TotalPages"]),
                            HasNext = Convert.ToBoolean(row["HasNext"]),
                            HasPrevious = Convert.ToBoolean(row["HasPrevious"])
                        };
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while performing enhanced search.", ex);
            }
        }

        /// <summary>
        /// Get search suggestions for autocomplete
        /// </summary>
        public SearchSuggestionResponse GetSearchSuggestions(SearchSuggestionRequest request)
        {
            try
            {
                return productRepository.GetSearchSuggestions(request);
            }
            catch (Exception ex)
            {
                return new SearchSuggestionResponse
                {
                    Success = false,
                    Message = "Error getting search suggestions",
                    Query = request.Query
                };
            }
        }

        /// <summary>
        /// Get filtered product list for admin management
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="payload">Filter parameters</param>
        /// <returns>Filtered product list with pagination</returns>
        public ProductSearchResponse GetProductListFiltered(string tenantId, ProductListFilteredRequest payload)
        {
            try
            {
                if (payload == null)
                    throw new ArgumentNullException(nameof(payload), "Filter payload cannot be null.");

                // Convert ProductListFilteredRequest to ProductSearchPayload
                var searchPayload = new ProductSearchPayload
                {
                    Page = payload.Page,
                    Limit = payload.Limit,
                    SortBy = payload.SortBy ?? "productName",
                    SortOrder = payload.SortOrder?.ToLower() ?? "asc",
                    Search = string.Empty, // No search text for filtered list
                    Category = !string.IsNullOrEmpty(payload.CategoryId) && int.TryParse(payload.CategoryId, out int categoryId) ? categoryId : null,
                    InStock = null,
                    BestSeller = null,
                    HasOffer = null,
                    MinPrice = null,
                    MaxPrice = null,
                    Rating = null
                };

                // Calculate offset for pagination
                int offset = (payload.Page - 1) * payload.Limit;

                // Call repository method to get filtered results
                var searchResults = productRepository.SearchProducts(tenantId, searchPayload, offset);

                // Map the results to response model
                var response = new ProductSearchResponse();

                if (searchResults != null && searchResults.Tables.Count >= 2)
                {
                    // First table contains product data
                    var productTable = searchResults.Tables[0];
                    // Second table contains total count
                    var countTable = searchResults.Tables[1];

                    // Map products
                    response.Products = MapProductSearchResults(productTable);

                    // Apply active filter if specified
                    if (payload.Active.HasValue)
                    {
                        response.Products = response.Products.Where(p => p.Active == payload.Active.Value).ToList();
                    }

                    // Map pagination info
                    int totalCount = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;
                    
                    // Adjust total count if active filter was applied
                    if (payload.Active.HasValue)
                    {
                        totalCount = response.Products.Count;
                    }

                    response.Pagination = new PaginationInfo
                    {
                        Page = payload.Page,
                        Limit = payload.Limit,
                        Total = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / payload.Limit),
                        HasNext = payload.Page * payload.Limit < totalCount,
                        HasPrevious = payload.Page > 1
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting filtered product list.", ex);
            }
        }

        /// <summary>
        /// Maps DataTable results to ProductDetailItem list
        /// </summary>
        /// <param name="dataTable">DataTable containing product data</param>
        /// <returns>List of ProductDetailItem</returns>
        private List<ProductDetailItem> MapProductSearchResults(DataTable dataTable)
        {
            var products = new List<ProductDetailItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                var product = new ProductDetailItem
                {
                    ProductId = GetColumnValue<long>(row, "ProductId"),
                    TenantId = GetColumnValue<long>(row, "TenantId"),
                    ProductName = GetColumnValue<string>(row, "ProductName", string.Empty),
                    ProductDescription = GetColumnValue<string>(row, "ProductDescription", string.Empty),
                    ProductCode = GetColumnValue<string>(row, "ProductCode", string.Empty),
                    FullDescription = GetColumnValue<string>(row, "FullDescription", string.Empty),
                    Specification = GetColumnValue<string>(row, "Specification", string.Empty),
                    Story = GetColumnValue<string>(row, "Story", string.Empty),
                    PackQuantity = GetColumnValue<int>(row, "PackQuantity"),
                    Quantity = GetColumnValue<int>(row, "Quantity"),
                    Total = GetColumnValue<int>(row, "Total"),
                    Price = GetColumnValue<decimal>(row, "Price"),
                    Category = GetColumnValue<int>(row, "Category"),
                    Rating = GetColumnValue<int>(row, "Rating"),
                    Active = GetColumnValue<bool>(row, "Active"),
                    Trending = GetColumnValue<int>(row, "Trending"),
                    UserBuyCount = GetColumnValue<int>(row, "UserBuyCount"),
                    Return = GetColumnValue<int>(row, "Return"),
                    Created = GetColumnValue<DateTime>(row, "Created"),
                    Modified = GetColumnValue<DateTime>(row, "Modified"),
                    InStock = GetColumnValue<bool>(row, "InStock"),
                    BestSeller = GetColumnValue<bool>(row, "BestSeller"),
                    DeliveryDate = GetColumnValue<int>(row, "DeliveryDate"),
                    Offer = GetColumnValue<string>(row, "Offer", string.Empty),
                    OrderBy = GetColumnValue<int>(row, "OrderBy"),
                    UserId = GetColumnValue<long>(row, "UserId"),
                    Overview = GetColumnValue<string>(row, "Overview", string.Empty),
                    LongDescription = GetColumnValue<string>(row, "LongDescription", string.Empty),
                    Images = new List<ProductSearchImageInfo>() // Images will be populated separately if needed
                };

                products.Add(product);
            }

            return products;
        }

        /// <summary>x
        /// Get user's shopping cart with full product details
        /// </summary>
        /// <param name="request">Cart request with user details</param>
        /// <returns>Complete cart information with products</returns>
        public async Task<Model.User.CartResponse> GetUserCart(Model.ProductCart.GetCartRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Get cart attempt for user: {request.UserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                var cartData = await this.productRepository.GetUserCart(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Cart retrieval successful for user: {request.UserId} - {cartData.Items.Count} items found");

                return cartData;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Cart retrieval failed - user not found: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Cart retrieval error for user {request?.UserId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the cart.", ex);
            }
        }

        /// <summary>
        /// Get user's wishlist items with product details
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Tenant ID (optional)</param>
        /// <returns>Wishlist items with product details</returns>
        public async Task<Model.WishList.WishlistItemsResponse> GetUserWishlistItems(long userId, long? tenantId = null)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Get wishlist items attempt for user: {userId}, tenant: {tenantId}");

                if (userId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                var wishlistData = await this.productRepository.GetUserWishlistItems(userId, tenantId);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Wishlist retrieval successful for user: {userId} - {wishlistData.Items.Count} items found");

                return wishlistData;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Wishlist retrieval failed - user not found: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Wishlist retrieval error for user {userId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the wishlist.", ex);
            }
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        /// <param name="request">Add to cart request</param>
        /// <returns>Cart item details and summary</returns>
        public async Task<Model.ProductCart.AddToCartResponse> AddItemToCart(long tenantId, Model.ProductCart.AddToCartRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Add to cart attempt for user: {request.UserId}, product: {request.ProductId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (request.ProductId <= 0)
                    throw new ArgumentException("Valid Product ID is required");

                // if (request.Quantity <= 0)
                //     throw new ArgumentException("Quantity must be greater than 0");

                var cartResponse = await this.productRepository.AddItemToCart(tenantId, request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Add to cart successful for user: {request.UserId}, product: {request.ProductId}");

                return cartResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Add to cart failed - product/user not found: {request?.ProductId}/{request?.UserId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Add to cart failed - business rule violation for user: {request?.UserId}, product: {request?.ProductId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Add to cart error for user {request?.UserId}, product {request?.ProductId}: {ex.Message}");
                throw new Exception("An error occurred while adding item to cart.", ex);
            }
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        /// <param name="request">Remove from cart request</param>
        /// <returns>Removal confirmation and updated cart summary</returns>
        public async Task<Model.ProductCart.RemoveFromCartResponse> RemoveItemFromCart(long tenantId, Model.ProductCart.RemoveFromCartRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Remove from cart attempt for user: {request.UserId}, product: {request.ProductId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (request.ProductId <= 0)
                    throw new ArgumentException("Valid Product ID is required");

                var removeResponse = await this.productRepository.RemoveItemFromCart(tenantId, request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Remove from cart successful for user: {request.UserId}, product: {request.ProductId}");

                return removeResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Remove from cart failed - product not found in cart: {request?.ProductId} for user: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Remove from cart error for user {request?.UserId}, product {request?.ProductId}: {ex.Message}");
                throw new Exception("An error occurred while removing item from cart.", ex);
            }
        }

        /// <summary>
        /// Clear entire cart
        /// </summary>
        /// <param name="request">Clear cart request</param>
        /// <returns>Cart clearing confirmation and statistics</returns>
        public async Task<Model.ProductCart.ClearCartResponse> ClearCart(long tenantId, Model.ProductCart.ClearCartRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Clear cart attempt for user: {request.UserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                var clearResponse = await this.productRepository.ClearCart(tenantId, request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Clear cart successful for user: {request.UserId} - {clearResponse.ClearedItemCount} items cleared");

                return clearResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Clear cart failed - user not found or cart already empty: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Clear cart error for user {request?.UserId}: {ex.Message}");
                throw new Exception("An error occurred while clearing the cart.", ex);
            }
        }

        /// <summary>
        /// Clear entire wishlist
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Clear wishlist request</param>
        /// <returns>Wishlist clearing confirmation and statistics</returns>
        public async Task<Model.WishList.ClearWishlistResponse> ClearWishlist(long tenantId, Model.WishList.ClearWishlistRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Clear wishlist attempt for user: {request.UserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                var clearResponse = await this.productRepository.ClearWishlist(tenantId, request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Clear wishlist successful for user: {request.UserId} - {clearResponse.ClearedItemCount} items cleared");

                return clearResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Clear wishlist failed - user not found or wishlist already empty: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Clear wishlist error for user {request?.UserId}: {ex.Message}");
                throw new Exception("An error occurred while clearing the wishlist.", ex);
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="request">Create order request</param>
        /// <returns>Order creation confirmation and details</returns>
        public async Task<Model.Order.CreateOrderResponse> CreateOrder(Model.Order.CreateOrderRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Create order attempt for user: {request.UserId} with {request.Items.Count} items");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (request.Items == null || !request.Items.Any())
                    throw new ArgumentException("Order must contain at least one item");

                if (request.ShippingAddress == null)
                    throw new ArgumentException("Shipping address is required");

                if (request.PaymentMethod == null)
                    throw new ArgumentException("Payment method is required");

                if (request.ShippingMethod == null)
                    throw new ArgumentException("Shipping method is required");

                if (request.Totals == null)
                    throw new ArgumentException("Order totals are required");

                // Validate each item has valid values
                foreach (var item in request.Items)
                {
                    if (item.ProductId <= 0)
                        throw new ArgumentException($"Invalid product ID: {item.ProductId}");

                    if (item.Quantity <= 0)
                        throw new ArgumentException($"Invalid quantity for product {item.ProductId}: {item.Quantity}");

                    if (item.Price <= 0)
                        throw new ArgumentException($"Invalid price for product {item.ProductId}: {item.Price}");

                    if (Math.Abs(item.Total - (item.Price * item.Quantity)) > 0.01m)
                        throw new ArgumentException($"Price calculation mismatch for product {item.ProductId}");
                }

                var orderResponse = await this.productRepository.CreateOrder(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Create order successful for user: {request.UserId}, Order Number: {orderResponse.OrderNumber}");

                await SendOrderConfirmationEmail(request, orderResponse);

                return orderResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Create order failed - user not found: {request?.UserId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Create order failed - business rule violation for user: {request?.UserId}");
                throw;
            }
            catch (ArgumentException)
            {
                throw; // Re-throw validation errors with their original message
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Create order error for user {request?.UserId}: {ex.Message}");
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogError($"Inner exception: {ex.InnerException.Message}");
                }
                // Return detailed error in development for debugging
                throw new Exception($"Order creation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user orders with pagination and filtering
        /// </summary>
        /// <param name="request">Get orders request</param>
        /// <returns>List of orders with pagination information</returns>
        public async Task<Model.Order.GetOrdersResponse> GetOrders(Model.Order.GetOrdersRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Get orders attempt for user: {request.UserId}, page: {request.Page}, limit: {request.Limit}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (request.Page < 1)
                    request.Page = 1;

                if (request.Limit < 1 || request.Limit > 100)
                    request.Limit = 10;

                var ordersResponse = await this.productRepository.GetOrders(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Get orders successful for user: {request.UserId}, found {ordersResponse.Orders.Count} orders");

                return ordersResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Get orders failed - user not found: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Get orders error for user {request?.UserId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving orders.", ex);
            }
        }

        /// <summary>
        /// Get order details by order ID
        /// </summary>
        /// <param name="request">Get order by ID request</param>
        /// <returns>Detailed order information</returns>
        public async Task<Model.Order.GetOrderByIdResponse> GetOrderById(Model.Order.GetOrderByIdRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Get order by ID attempt for user: {request.UserId}, order: {request.OrderId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (request.OrderId <= 0)
                    throw new ArgumentException("Valid Order ID is required");

                var orderResponse = await this.productRepository.GetOrderById(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Get order by ID successful for user: {request.UserId}, order: {request.OrderId}");

                return orderResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Get order by ID failed - order not found or doesn't belong to user: {request?.OrderId} for user: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Get order by ID error for user {request?.UserId}, order {request?.OrderId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the order.", ex);
            }
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="request">Cancel order request</param>
        /// <returns>Order cancellation confirmation</returns>
        public async Task<Model.Order.CancelOrderResponse> CancelOrder(Model.Order.CancelOrderRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Cancel order attempt for user: {request.UserId}, order: {request.OrderId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (request.OrderId <= 0)
                    throw new ArgumentException("Valid Order ID is required");

                var cancelResponse = await this.productRepository.CancelOrder(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Cancel order successful for user: {request.UserId}, order: {request.OrderId}");

                return cancelResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Cancel order failed - order not found or doesn't belong to user: {request?.OrderId} for user: {request?.UserId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Cancel order failed - order cannot be cancelled: {request?.OrderId} for user: {request?.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Cancel order error for user {request?.UserId}, order {request?.OrderId}: {ex.Message}");
                throw new Exception("An error occurred while cancelling the order.", ex);
            }
        }

        private async Task SendOrderConfirmationEmail(Model.Order.CreateOrderRequest request, Model.Order.CreateOrderResponse response)
        {
            try
            {
                var toEmail = request?.ShippingAddress?.Email;
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogWarning("Order confirmation email skipped: recipient email is missing.");
                    return;
                }

                var customerName = $"{request?.ShippingAddress?.FirstName} {request?.ShippingAddress?.LastName}".Trim();
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    customerName = $"{request?.ShippingAddress?.FirstName} {request?.ShippingAddress?.LastName}".Trim();
                }

                var companyName = _configuration["Invoice:CompanyName"] ?? _configuration["Email:FromName"] ?? "xtraCHEF";
                var shippingAddress = FormatAddress(request?.ShippingAddress);
                var orderDate = response.CreatedDate == default ? DateTime.UtcNow : response.CreatedDate;

                var emailRequest = new SendEmailRequest
                {
                    To = toEmail,
                    Subject = $"Order Confirmation - {response.OrderNumber}",
                    TemplateName = "OrderConfirmation",
                    TemplateData = new Dictionary<string, object>
                    {
                        { "CompanyName", companyName },
                        { "CustomerName", string.IsNullOrWhiteSpace(customerName) ? "Customer" : customerName },
                        { "OrderNumber", response.OrderNumber ?? response.OrderId.ToString() },
                        { "OrderDate", orderDate.ToString("dd MMM yyyy") },
                        { "ItemCount", response.ItemCount },
                        { "TotalAmount", $"₹{response.TotalAmount:F2}" },
                        { "OrderStatus", response.OrderStatus ?? "Pending" },
                        { "PaymentStatus", response.PaymentStatus ?? "Pending" },
                        { "ShippingAddress", shippingAddress }
                    }
                };

                await emailService.SendEmail(emailRequest);
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Order confirmation email failed: {ex.Message}");
            }
        }

        private async Task SendShippingUpdateEmail(Model.Order.UpdateOrderStatusRequest request, Model.Order.UpdateOrderStatusResponse response)
        {
            try
            {
                var newStatus = response?.NewStatus ?? request?.Status;
                if (string.IsNullOrWhiteSpace(newStatus))
                {
                    return;
                }

                if (!string.Equals(newStatus, "Shipped", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(newStatus, "Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var userId = response?.UserId > 0 ? response.UserId : (request?.UserId ?? 0);
                if (userId <= 0)
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogWarning("Shipping update email skipped: userId not available.");
                    return;
                }

                var userList = userRepository.GetUser("SP_CUSTOMER", userId);
                var user = userList?.FirstOrDefault();
                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Shipping update email skipped: email not found for user {userId}.");
                    return;
                }

                var customerName = $"{user.FirstName} {user.LastName}".Trim();
                var companyName = _configuration["Invoice:CompanyName"] ?? _configuration["Email:FromName"] ?? "xtraCHEF";
                var trackingNumber = response?.TrackingNumber ?? request?.TrackingNumber ?? "N/A";
                var carrier = response?.Carrier ?? request?.Carrier ?? "N/A";
                var estimatedDelivery = response?.EstimatedDelivery ?? request?.EstimatedDelivery;
                var note = response?.StatusNote ?? request?.Note ?? string.Empty;

                var emailRequest = new SendEmailRequest
                {
                    To = user.Email,
                    Subject = $"Your order {response?.OrderNumber ?? request?.OrderId.ToString()} is {newStatus}",
                    TemplateName = "ShippingUpdate",
                    TemplateData = new Dictionary<string, object>
                    {
                        { "CompanyName", companyName },
                        { "CustomerName", string.IsNullOrWhiteSpace(customerName) ? "Customer" : customerName },
                        { "OrderNumber", response?.OrderNumber ?? request?.OrderId.ToString() ?? "-" },
                        { "Status", newStatus },
                        { "TrackingNumber", trackingNumber },
                        { "Carrier", carrier },
                        { "EstimatedDelivery", estimatedDelivery.HasValue ? estimatedDelivery.Value.ToString("dd MMM yyyy") : "TBD" },
                        { "Note", note }
                    }
                };

                await emailService.SendEmail(emailRequest);
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Shipping update email failed: {ex.Message}");
            }
        }

        private static string FormatAddress(Model.Order.AddressRequest address)
        {
            if (address == null)
            {
                return "Address not available";
            }

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(address.Company)) parts.Add(address.Company);
            if (!string.IsNullOrWhiteSpace(address.Address1)) parts.Add(address.Address1);
            if (!string.IsNullOrWhiteSpace(address.Address2)) parts.Add(address.Address2);
            if (!string.IsNullOrWhiteSpace(address.City)) parts.Add(address.City);
            if (!string.IsNullOrWhiteSpace(address.State)) parts.Add(address.State);
            if (!string.IsNullOrWhiteSpace(address.ZipCode)) parts.Add(address.ZipCode);
            if (!string.IsNullOrWhiteSpace(address.Country)) parts.Add(address.Country);

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="request">Update order status request</param>
        /// <returns>Order status update confirmation</returns>
        public async Task<Model.Order.UpdateOrderStatusResponse> UpdateOrderStatus(Model.Order.UpdateOrderStatusRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Update order status attempt for order: {request.OrderId} to status: {request.Status}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.OrderId <= 0)
                    throw new ArgumentException("Valid Order ID is required");

                if (string.IsNullOrEmpty(request.Status))
                    throw new ArgumentException("Status is required");

                // Validate status value (optional - could also be done in stored procedure)
                var validStatuses = new[] { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled", "Returned", "Refunded" };
                if (!validStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Invalid status: {request.Status}. Valid statuses are: {string.Join(", ", validStatuses)}");
                }

                var statusResponse = await this.productRepository.UpdateOrderStatus(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Update order status successful for order: {request.OrderId} to status: {request.Status}");

                await SendShippingUpdateEmail(request, statusResponse);

                return statusResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Update order status failed - order not found: {request?.OrderId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Update order status failed - invalid status transition: {request?.OrderId} to {request?.Status}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Update order status error for order {request?.OrderId}: {ex.Message}");
                throw new Exception("An error occurred while updating the order status.", ex);
            }
        }

        /// <summary>
        /// Bulk update order status for multiple orders
        /// </summary>
        /// <param name="request">Bulk update order status request</param>
        /// <returns>Bulk update result</returns>
        public async Task<Model.Order.BulkUpdateOrderStatusResponse> BulkUpdateOrderStatus(Model.Order.BulkUpdateOrderStatusRequest request)
        {
            var response = new Model.Order.BulkUpdateOrderStatusResponse();

            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Bulk update order status attempt for {request.OrderIds.Count} orders to status: {request.Status}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.OrderIds == null || request.OrderIds.Count == 0)
                    throw new ArgumentException("At least one Order ID is required");

                if (string.IsNullOrEmpty(request.Status))
                    throw new ArgumentException("Status is required");

                // Validate status value
                var validStatuses = new[] { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled", "Returned", "Refunded" };
                if (!validStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Invalid status: {request.Status}. Valid statuses are: {string.Join(", ", validStatuses)}");
                }

                // Update each order
                foreach (var orderId in request.OrderIds)
                {
                    try
                    {
                        var updateRequest = new Model.Order.UpdateOrderStatusRequest
                        {
                            OrderId = orderId,
                            Status = request.Status,
                            Note = request.Note,
                            TenantId = request.TenantId,
                            UpdatedBy = request.AdminUserId,
                            IpAddress = request.IpAddress,
                            UserAgent = request.UserAgent
                        };

                        var statusResponse = await this.productRepository.UpdateOrderStatus(updateRequest);
                        response.Orders.Add(statusResponse);
                        response.Updated++;
                    }
                    catch (Exception ex)
                    {
                        response.Failed++;
                        response.Errors.Add($"Order {orderId}: {ex.Message}");
                        this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Failed to update order {orderId}: {ex.Message}");
                    }
                }

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Bulk update order status completed: {response.Updated} updated, {response.Failed} failed");

                return response;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Bulk update order status error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <param name="request">Get all users request</param>
        /// <returns>Users list with pagination</returns>
        public async Task<Model.Admin.GetAllUsersResponse> GetAllUsers(Model.Admin.GetAllUsersRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Admin get all users attempt by admin: {request.AdminUserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.AdminUserId <= 0)
                    throw new ArgumentException("Valid Admin User ID is required");

                if (request.Page < 1)
                    request.Page = 1;

                if (request.Limit < 1 || request.Limit > 100)
                    request.Limit = 10;

                var usersResponse = await this.productRepository.GetAllUsers(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Admin get all users successful by admin: {request.AdminUserId}, found {usersResponse.Users.Count} users");

                return usersResponse;
            }
            catch (UnauthorizedAccessException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Admin get all users failed - insufficient privileges: {request?.AdminUserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Admin get all users error for admin {request?.AdminUserId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }

        /// <summary>
        /// Update user role (Admin only)
        /// </summary>
        /// <param name="request">Update user role request</param>
        /// <returns>Role update confirmation</returns>
        public async Task<Model.Admin.UpdateUserRoleResponse> UpdateUserRole(Model.Admin.UpdateUserRoleRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Admin update user role attempt by admin: {request.AdminUserId} for user: {request.UserId} to role: {request.Role}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.AdminUserId <= 0)
                    throw new ArgumentException("Valid Admin User ID is required");

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (string.IsNullOrEmpty(request.Role))
                    throw new ArgumentException("Role is required");

                // Validate role value (optional - could also be done in stored procedure)
                var validRoles = new[] { "Customer", "Executive", "Admin", "SuperAdmin", "Manager", "Support" };
                if (!validRoles.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Invalid role: {request.Role}. Valid roles are: {string.Join(", ", validRoles)}");
                }

                // Validate permissions if provided
                if (request.Permissions != null && request.Permissions.Any())
                {
                    var validPermissions = new[] { 
                        "view_products", "manage_products", "view_orders", "manage_orders", 
                        "view_users", "manage_users", "view_reports", "manage_settings",
                        "view_inventory", "manage_inventory", "view_analytics", "manage_roles"
                    };
                    
                    var invalidPermissions = request.Permissions.Where(p => !validPermissions.Contains(p, StringComparer.OrdinalIgnoreCase)).ToList();
                    if (invalidPermissions.Any())
                    {
                        throw new ArgumentException($"Invalid permissions: {string.Join(", ", invalidPermissions)}");
                    }
                }

                var roleResponse = await this.productRepository.UpdateUserRole(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Admin update user role successful by admin: {request.AdminUserId} for user: {request.UserId} to role: {request.Role}");

                return roleResponse;
            }
            catch (UnauthorizedAccessException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Admin update user role failed - insufficient privileges: {request?.AdminUserId}");
                throw;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Admin update user role failed - user not found: {request?.UserId} by admin: {request?.AdminUserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Admin update user role error by admin {request?.AdminUserId} for user {request?.UserId}: {ex.Message}");
                throw new Exception("An error occurred while updating user role.", ex);
            }
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        /// <param name="request">Get all orders request</param>
        /// <returns>Orders list with pagination and statistics</returns>
        public async Task<Model.Admin.GetAllOrdersResponse> GetAllOrders(Model.Admin.GetAllOrdersRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Admin get all orders attempt by admin: {request.AdminUserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.AdminUserId <= 0)
                    throw new ArgumentException("Valid Admin User ID is required");

                if (request.Page < 1)
                    request.Page = 1;

                if (request.Limit < 1 || request.Limit > 100)
                    request.Limit = 10;

                // Validate date range
                if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate > request.EndDate)
                    throw new ArgumentException("Start date cannot be later than end date");

                // Validate status if provided
                if (!string.IsNullOrEmpty(request.Status))
                {
                    var validStatuses = new[] { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled", "Returned", "Refunded" };
                    if (!validStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException($"Invalid status: {request.Status}. Valid statuses are: {string.Join(", ", validStatuses)}");
                    }
                }

                var ordersResponse = await this.productRepository.GetAllOrders(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Admin get all orders successful by admin: {request.AdminUserId}, found {ordersResponse.Orders.Count} orders");

                return ordersResponse;
            }
            catch (UnauthorizedAccessException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Admin get all orders failed - insufficient privileges: {request?.AdminUserId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Admin get all orders error for admin {request?.AdminUserId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving orders.", ex);
            }
        }

        /// <summary>
        /// Add multiple images to a product
        /// </summary>
        /// <param name="request">Add product images request</param>
        /// <returns>List of added images</returns>
        public async Task<Model.Product.AddProductImagesResponse> AddProductImages(Model.Product.AddProductImagesRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Add product images attempt for product: {request.ProductId}, count: {request.Images?.Count ?? 0}");
                
                // Log each file being processed
                if (request.Images != null)
                {
                    for (int i = 0; i < request.Images.Count; i++)
                    {
                        var file = request.Images[i];
                        this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Service: Processing file {i + 1}: {file?.FileName}, Size: {file?.Length}, ContentType: {file?.ContentType}");
                    }
                }

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.ProductId <= 0)
                    throw new ArgumentException("Valid Product ID is required");

                if (request.Images == null || !request.Images.Any())
                    throw new ArgumentException("At least one image file is required");

                // Validate and process each image
                var imageDataList = new List<Tenant.Query.Model.Product.ImageUploadData>();
                int orderCounter = request.OrderBy;

                foreach (var file in request.Images)
                {
                    // Validate file
                    if (file == null || file.Length == 0)
                        throw new ArgumentException($"Invalid file uploaded");

                    // Validate file size
                    if (file.Length > 10 * 1024 * 1024) // 10MB
                        throw new ArgumentException($"File {file.FileName} exceeds size limit");

                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(file.ContentType.ToLower()))
                        throw new ArgumentException($"File {file.FileName} has invalid type. Allowed: JPEG, PNG, GIF, WebP");

                    // Read file data
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }

                    // Create thumbnail if image is large enough
                    byte[] thumbnailData = null;
                    try
                    {
                        this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Service: Creating thumbnail for {file.FileName}, original size: {imageData.Length} bytes");
                        thumbnailData = await CreateThumbnailAsync(imageData, 200, 200);
                        this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Service: Thumbnail created successfully for {file.FileName}, thumbnail size: {thumbnailData?.Length ?? 0} bytes");
                    }
                    catch (Exception ex)
                    {
                        this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Failed to create thumbnail for {file.FileName}: {ex.Message}");
                        this._loggerFactory.CreateLogger<ProductService>().LogError(ex, $"Thumbnail creation error details for {file.FileName}");
                    }

                    // Convert to base64
                    string imageDataBase64 = Convert.ToBase64String(imageData);
                    string thumbnailDataBase64 = thumbnailData != null ? Convert.ToBase64String(thumbnailData) : null;
                    
                    this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Service: Base64 conversion - Image: {imageDataBase64?.Length ?? 0} chars, Thumbnail: {thumbnailDataBase64?.Length ?? 0} chars for {file.FileName}");
                    
                    // Add to upload list
                    imageDataList.Add(new Tenant.Query.Model.Product.ImageUploadData
                    {
                        ImageName = file.FileName,
                        ContentType = file.ContentType,
                        FileSize = file.Length,
                        ImageData = imageDataBase64,
                        ThumbnailData = thumbnailDataBase64,
                        IsMain = request.Main && imageDataList.Count == 0, // Only first image can be main if requested
                        OrderBy = orderCounter++
                    });
                }

                var imagesResponse = await this.productRepository.AddProductImages(request, imageDataList);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Add product images successful for product: {request.ProductId}, added: {imagesResponse.TotalAdded}");

                return imagesResponse;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Add product images error for product {request?.ProductId}: {ex.Message}");
                throw new Exception("An error occurred while adding product images.", ex);
            }
        }

        /// <summary>
        /// Update product image properties
        /// </summary>
        /// <param name="request">Update product image request</param>
        /// <returns>Updated image information</returns>
        public async Task<Model.Product.UpdateProductImageResponse> UpdateProductImage(Model.Product.UpdateProductImageRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Update product image attempt for product: {request.ProductId}, image: {request.ImageId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.ProductId <= 0)
                    throw new ArgumentException("Valid Product ID is required");

                if (request.ImageId <= 0)
                    throw new ArgumentException("Valid Image ID is required");

                // Validate that at least one property is being updated
                if (!request.Main.HasValue && !request.Active.HasValue && !request.OrderBy.HasValue)
                    throw new ArgumentException("At least one property (main, active, or orderBy) must be specified for update");

                // Validate OrderBy if provided
                if (request.OrderBy.HasValue && request.OrderBy.Value < 0)
                    throw new ArgumentException("OrderBy must be a non-negative number");

                var imageResponse = await this.productRepository.UpdateProductImage(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Update product image successful for product: {request.ProductId}, image: {request.ImageId}");

                return imageResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Update product image failed - image not found: {request?.ImageId} for product: {request?.ProductId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Update product image error for product {request?.ProductId}, image {request?.ImageId}: {ex.Message}");
                throw new Exception("An error occurred while updating the product image.", ex);
            }
        }

        /// <summary>
        /// Set main product image
        /// </summary>
        /// <param name="request">Set main product image request</param>
        /// <returns>Updated image information</returns>
        public async Task<Model.Product.SetMainProductImageResponse> SetMainProductImage(Model.Product.SetMainProductImageRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Set main product image attempt for product: {request.ProductId}, image: {request.ImageId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.ProductId <= 0)
                    throw new ArgumentException("Valid Product ID is required");

                if (request.ImageId <= 0)
                    throw new ArgumentException("Valid Image ID is required");

                var imageResponse = await this.productRepository.SetMainProductImage(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Set main product image successful for product: {request.ProductId}, image: {request.ImageId}");

                return imageResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Set main product image failed - image not found: {request?.ImageId} for product: {request?.ProductId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Set main product image error for product {request?.ProductId}, image {request?.ImageId}: {ex.Message}");
                throw new Exception("An error occurred while setting the main product image.", ex);
            }
        }

        /// <summary>
        /// Delete product image
        /// </summary>
        /// <param name="request">Delete product image request</param>
        /// <returns>Deletion confirmation and remaining images</returns>
        public async Task<Model.Product.DeleteProductImageResponse> DeleteProductImage(Model.Product.DeleteProductImageRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Delete product image attempt for product: {request.ProductId}, image: {request.ImageId}, hard: {request.HardDelete}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.ProductId <= 0)
                    throw new ArgumentException("Valid Product ID is required");

                if (request.ImageId <= 0)
                    throw new ArgumentException("Valid Image ID is required");

                var deleteResponse = await this.productRepository.DeleteProductImage(request);

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Delete product image successful for product: {request.ProductId}, image: {request.ImageId}");

                return deleteResponse;
            }
            catch (KeyNotFoundException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Delete product image failed - image not found: {request?.ImageId} for product: {request?.ProductId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Delete product image failed - cannot delete last main image: {request?.ImageId} for product: {request?.ProductId}");
                throw;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Delete product image error for product {request?.ProductId}, image {request?.ImageId}: {ex.Message}");
                throw new Exception("An error occurred while deleting the product image.", ex);
            }
        }

        #endregion

        #region Razorpay Payment Methods

        /// <summary>
        /// Create Razorpay hosted checkout configuration
        /// </summary>
        /// <param name="request">Razorpay hosted checkout request</param>
        /// <returns>Razorpay hosted checkout response with checkout configuration</returns>
        public async Task<Model.Order.RazorpayHostedCheckoutResponse> CreateRazorpayHostedCheckout(Model.Order.RazorpayHostedCheckoutRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Creating Razorpay hosted checkout for amount: {request.Amount}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.Amount <= 0)
                    throw new ArgumentException("Amount must be greater than 0");

                // Get Razorpay configuration
                var razorpayKeyId = _configuration["Razorpay:KeyId"];
                var razorpayKeySecret = _configuration["Razorpay:KeySecret"];

                if (string.IsNullOrEmpty(razorpayKeyId) || string.IsNullOrEmpty(razorpayKeySecret))
                    throw new InvalidOperationException("Razorpay configuration is missing");

                // Create Razorpay order
                var orderId = await CreateRazorpayOrderAsync(request.Amount, request.Currency ?? "INR", request.Receipt, razorpayKeyId, razorpayKeySecret);

                // Generate session token for security
                var sessionToken = Guid.NewGuid().ToString("N");
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Create checkout configuration for Razorpay Checkout.js
                var checkoutConfig = new
                {
                    key = razorpayKeyId,
                    amount = request.Amount,
                    currency = request.Currency ?? "INR",
                    name = "Xtrachef",
                    description = "Order Payment",
                    order_id = orderId,
                    prefill = new
                    {
                        name = request.CustomerName ?? "",
                        email = request.CustomerEmail ?? "",
                        contact = request.CustomerContact ?? ""
                    },
                    theme = new
                    {
                        color = "#22c55e"
                    }
                };

                var response = new Model.Order.RazorpayHostedCheckoutResponse
                {
                    OrderId = orderId,
                    CheckoutUrl = System.Text.Json.JsonSerializer.Serialize(checkoutConfig),
                    Amount = request.Amount,
                    Timestamp = timestamp,
                    SessionToken = sessionToken,
                    RazorpayKeyId = razorpayKeyId
                };

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Razorpay hosted checkout created successfully. OrderId: {orderId}");

                return response;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Error creating Razorpay hosted checkout: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verify Razorpay payment signature
        /// </summary>
        /// <param name="request">Razorpay payment verification request</param>
        /// <returns>Verification result</returns>
        public async Task<Model.Order.RazorpayPaymentVerificationResponse> VerifyRazorpayPayment(Model.Order.RazorpayPaymentVerificationRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Verifying Razorpay payment. OrderId: {request.OrderId}, PaymentId: {request.PaymentId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrEmpty(request.OrderId))
                    throw new ArgumentException("Order ID is required");

                if (string.IsNullOrEmpty(request.PaymentId))
                    throw new ArgumentException("Payment ID is required");

                if (string.IsNullOrEmpty(request.Signature))
                    throw new ArgumentException("Signature is required");

                // Get Razorpay secret
                var razorpayKeySecret = _configuration["Razorpay:KeySecret"];

                if (string.IsNullOrEmpty(razorpayKeySecret))
                    throw new InvalidOperationException("Razorpay configuration is missing");

                // Verify signature
                var isValid = VerifyRazorpaySignature(request.OrderId, request.PaymentId, request.Signature, razorpayKeySecret);

                if (!isValid)
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Razorpay signature verification failed for PaymentId: {request.PaymentId}");
                    return new Model.Order.RazorpayPaymentVerificationResponse
                    {
                        Success = false,
                        Message = "Payment signature verification failed",
                        OrderId = request.OrderId,
                        PaymentId = request.PaymentId
                    };
                }

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Razorpay payment verified successfully. PaymentId: {request.PaymentId}");

                return new Model.Order.RazorpayPaymentVerificationResponse
                {
                    Success = true,
                    Message = "Payment verified successfully",
                    OrderId = request.OrderId,
                    PaymentId = request.PaymentId,
                    Amount = request.Amount,
                    Currency = "INR",
                    Status = "captured",
                    VerifiedAt = DateTime.UtcNow,
                    Id = request.OrderIdInternal
                };
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Error verifying Razorpay payment: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verify Razorpay payments (alternate endpoint)
        /// </summary>
        /// <param name="request">Razorpay payments verification request</param>
        /// <returns>Verification result</returns>
        public async Task<Model.Order.VerifyRazorpayPaymentsResponse> VerifyRazorpayPayments(Model.Order.VerifyRazorpayPaymentsRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Verifying Razorpay payments. OrderId: {request.OrderId}, PaymentId: {request.PaymentId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrEmpty(request.OrderId))
                    throw new ArgumentException("Order ID is required");

                if (string.IsNullOrEmpty(request.PaymentId))
                    throw new ArgumentException("Payment ID is required");

                if (string.IsNullOrEmpty(request.Signature))
                    throw new ArgumentException("Signature is required");

                // Get Razorpay secret
                var razorpayKeySecret = _configuration["Razorpay:KeySecret"];

                if (string.IsNullOrEmpty(razorpayKeySecret))
                    throw new InvalidOperationException("Razorpay configuration is missing");

                // Verify signature
                var isValid = VerifyRazorpaySignature(request.OrderId, request.PaymentId, request.Signature, razorpayKeySecret);

                if (!isValid)
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Razorpay signature verification failed for PaymentId: {request.PaymentId}");
                    return new Model.Order.VerifyRazorpayPaymentsResponse
                    {
                        Success = false,
                        Message = "Payment signature verification failed",
                        VerifiedOrderId = request.OrderId
                    };
                }

                this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Razorpay payments verified successfully. PaymentId: {request.PaymentId}");

                // Update order status to "paid" after successful verification
                string paymentStatus = "paid";
                string orderStatus = "confirmed";
                
                if (request.InternalOrderId.HasValue || !string.IsNullOrEmpty(request.InternalOrderNumber))
                {
                    try
                    {
                        var updateRequest = new Model.Order.UpdateOrderStatusWithPaymentRequest
                        {
                            OrderId = request.InternalOrderId,
                            OrderNumber = request.InternalOrderNumber,
                            Status = orderStatus,
                            PaymentStatus = paymentStatus,
                            RazorpayPaymentId = request.PaymentId,
                            RazorpayOrderId = request.OrderId,
                            RazorpaySignature = request.Signature,
                            Notes = "Payment verified successfully via Razorpay"
                        };
                        
                        var updateResult = await this.productRepository.UpdateOrderStatusWithPayment(updateRequest);
                        
                        if (updateResult != null)
                        {
                            paymentStatus = updateResult.PaymentStatus ?? paymentStatus;
                            orderStatus = updateResult.Status ?? orderStatus;
                            this._loggerFactory.CreateLogger<ProductService>().LogInformation($"Order status updated successfully. OrderId: {updateResult.OrderId}, PaymentStatus: {paymentStatus}, OrderStatus: {orderStatus}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail verification if status update fails
                        this._loggerFactory.CreateLogger<ProductService>().LogWarning($"Payment verified but failed to update order status: {ex.Message}");
                    }
                }

                return new Model.Order.VerifyRazorpayPaymentsResponse
                {
                    Success = true,
                    Message = "Payment verified successfully",
                    VerifiedOrderId = request.OrderId,
                    VerifiedAmount = request.Amount,
                    VerifiedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    OrderCreated = true,
                    OrderNumber = request.InternalOrderNumber,
                    OrderId = request.InternalOrderId,
                    Amount = request.Amount,
                    PaymentStatus = paymentStatus,
                    OrderStatus = orderStatus
                };
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Error verifying Razorpay payments: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Mark payment as failed or cancelled
        /// </summary>
        public async Task<Model.Order.MarkPaymentFailedResponse> MarkPaymentFailed(Model.Order.MarkPaymentFailedRequest request)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>()
                    .LogInformation($"Marking payment as {request.Reason}. RazorpayOrderId: {request.RazorpayOrderId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrEmpty(request.RazorpayOrderId))
                    throw new ArgumentException("Razorpay Order ID is required");

                if (string.IsNullOrEmpty(request.Reason) || (request.Reason != "cancelled" && request.Reason != "failed"))
                    throw new ArgumentException("Reason must be either 'cancelled' or 'failed'");

                // Determine order status based on reason
                string orderStatus = request.Reason == "cancelled" ? "cancelled" : "failed";
                string paymentStatus = "failed";

                // Update order status to failed/cancelled
                var updateRequest = new Model.Order.UpdateOrderStatusWithPaymentRequest
                {
                    RazorpayOrderId = request.RazorpayOrderId,
                    Status = orderStatus,
                    PaymentStatus = paymentStatus,
                    Notes = $"Payment {request.Reason}: {request.ReasonDescription ?? "User cancelled or payment failed"}"
                };

                var updateResult = await this.productRepository.UpdateOrderStatusWithPayment(updateRequest);

                // Handle case where order doesn't exist yet (payment cancelled before order creation)
                if (updateResult == null)
                {
                    this._loggerFactory.CreateLogger<ProductService>()
                        .LogWarning($"Payment marked as {request.Reason} but order not found. RazorpayOrderId: {request.RazorpayOrderId}");
                    
                    // Return success response even if order doesn't exist (this is valid for cancellations)
                    return new Model.Order.MarkPaymentFailedResponse
                    {
                        Success = true,
                        Message = $"Payment marked as {request.Reason} (order not created yet)",
                        OrderId = null,
                        OrderNumber = null,
                        PaymentStatus = paymentStatus
                    };
                }

                this._loggerFactory.CreateLogger<ProductService>()
                    .LogInformation($"Payment marked as {request.Reason} successfully. OrderId: {updateResult.OrderId}");

                return new Model.Order.MarkPaymentFailedResponse
                {
                    Success = true,
                    Message = $"Payment marked as {request.Reason}",
                    OrderId = updateResult.OrderId,
                    OrderNumber = updateResult.OrderNumber,
                    PaymentStatus = updateResult.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>()
                    .LogError($"Error marking payment as failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get payment status for a Razorpay order
        /// </summary>
        public async Task<Model.Order.PaymentStatusResponse> GetPaymentStatus(string razorpayOrderId)
        {
            try
            {
                this._loggerFactory.CreateLogger<ProductService>()
                    .LogInformation($"Getting payment status for Razorpay Order ID: {razorpayOrderId}");

                if (string.IsNullOrEmpty(razorpayOrderId))
                    throw new ArgumentException("Razorpay Order ID is required");

                var result = await this.productRepository.GetPaymentStatus(razorpayOrderId);

                this._loggerFactory.CreateLogger<ProductService>()
                    .LogInformation($"Payment status retrieved for Razorpay Order ID: {razorpayOrderId}, Status: {result.PaymentStatus}");

                return result;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>()
                    .LogError($"Error getting payment status: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create Razorpay order via API
        /// </summary>
        private async Task<string> CreateRazorpayOrderAsync(long amount, string currency, string receipt, string keyId, string keySecret)
        {
            try
            {
                using var httpClient = new System.Net.Http.HttpClient();

                // Set up basic authentication
                var authString = $"{keyId}:{keySecret}";
                var authBytes = Encoding.ASCII.GetBytes(authString);
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                // Create order payload
                var orderPayload = new
                {
                    amount = amount,
                    currency = currency,
                    receipt = receipt ?? $"rcpt_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };

                var content = new System.Net.Http.StringContent(
                    System.Text.Json.JsonSerializer.Serialize(orderPayload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    this._loggerFactory.CreateLogger<ProductService>().LogError($"Razorpay API error: {responseContent}");
                    throw new InvalidOperationException($"Failed to create Razorpay order: {responseContent}");
                }

                // Parse response to get order ID
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                var orderId = jsonDoc.RootElement.GetProperty("id").GetString();

                return orderId;
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Error creating Razorpay order: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verify Razorpay signature using HMAC SHA256
        /// </summary>
        private bool VerifyRazorpaySignature(string orderId, string paymentId, string signature, string secret)
        {
            try
            {
                // Create the signature data: order_id|payment_id
                var signatureData = $"{orderId}|{paymentId}";
                
                // Compute HMAC SHA256
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureData));
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

                // Compare signatures
                return string.Equals(computedSignature, signature.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                this._loggerFactory.CreateLogger<ProductService>().LogError($"Error verifying signature: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
