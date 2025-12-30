using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Repository;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Product;
using Microsoft.Extensions.Configuration;
using Sa.Common.ADO.DataAccess;
using Microsoft.CodeAnalysis;
using Tenant.Query.Uitility;
using SixLabors.ImageSharp;
using Tenant.Query.Model.WishList;
using Tenant.Query.Model.ProductCart;
using Tenant.Query.Model.Constant;
using Tenant.API.Base.Model.Validation;

namespace Tenant.Query.Repository.Product
{
    public class ProductRepository : TnBaseQueryRepository<Model.Product.ProductMaster, Context.Product.ProductContext>
    {

        #region Variable
        DataAccess _dataAccess;
        private readonly Sa.Common.ADO.DataAccess.DataAccess _dataAccessCmd;
        private string dbConnectionString = string.Empty;
        #endregion

        public ProductRepository(ProductContext dbContext,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            DataAccess dataAccess) : base(dbContext, loggerFactory)
        {
            dbConnectionString = this.DbContext.Database.GetDbConnection().ConnectionString;
            _dataAccess = dataAccess;
            _dataAccessCmd = dataAccess;
        }

        #region Get End Point
        /// <summary>
        /// s               
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="spName"></param>
        /// <returns></returns>
        public List<Model.Product.ProductCategory> GetMenuMaster(string tenantId, string spName)
        {
            try
            {
                //Executing query
                List<Model.Product.ProductCategory> productCategories = _dataAccess.ExecuteGenericList<Model.Product.ProductCategory>(spName, Convert.ToInt64(tenantId));

                return productCategories;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        #endregion 

        #region Example methods
        public override Task<ProductMaster> GetById(string tenantId, string id)
        {
            throw new NotImplementedException();
        }

        public DataTable VendorLookupById(Int64 tenantVendorId, Int64 tenantID)
        {
            try
            {
                return _dataAccess.ExecuteDataTable(Model.Constant.Constant.StoredProcedures.SA_REALTIMEOCR_GET_TENANT_VENDOR_DETAIL, tenantVendorId, tenantID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Remove cart (legacy method - use RemoveItemFromCart instead)
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="removeCartPayLoad">Remove cart payload</param>
        /// <returns>Return value from stored procedure</returns>
        public long RemoveProductCart(string tenantId, RemoveCartPayLoad removeCartPayLoad)
        {
            try
            {
                var cmd = _dataAccess.ExecuteNonQueryCMD(Model.Constant.Constant.StoredProcedures.SP_REMOVE_CART,
                    removeCartPayLoad.UserId,
                    removeCartPayLoad.ProductId);
                return Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove cart
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="orgImageName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long RemoveProductWishList(string tenantId, RemoveWhishListPayload removeWhishListPayload)
        {
            try
            {
                var cmd = _dataAccess.ExecuteNonQueryCMD(Model.Constant.Constant.StoredProcedures.SP_REMOVE_WISHLIST,
                    removeWhishListPayload.UserId,
                    removeWhishListPayload.ProductId);
                return Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // public long InsertImage(string imageName, string orgImageName, long userId)
        // {
        //     try
        //     {
        //         var result = await_dataAccess.ExecuteDataset(Model.Constant.Constant.StoredProcedures.SP_ADD_IMAGES, imageName, orgImageName, userId));
        //         return Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value.ToString());
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }

        ///// <summary>
        ///// Add Product
        ///// </summary>
        ///// <param name="tenantId"></param>
        ///// <param name="product"></param>
        ///// <returns></returns>
        //public async Task<long> AddProduct(long tenantId, Model.Product.Product product)
        //{
        //    try
        //    {
        //        var productId = await Task.Run(() =>
        //            _dataAccess.ExecuteDataTable(Model.Constant.Constant.StoredProcedures.SA_REALTIMEOCR_ADD_INVOICE,
        //            tenantId, 
        //            product.ProductName,
        //            product.Displayname,
        //            product.Rating,
        //            product.Total,
        //            product.Price,
        //            product.Tax,
        //            product.Stock,
        //            product.Description,
        //            product.Quantity,
        //            product.MinimunQuantity,
        //            product.Numofreviews,
        //            product.Active,
        //            1,
        //            1,
        //            product.BestSeller,
        //            product.VendorId));

        //        if (productId.Rows.Count > 0)
        //        {
        //            return Convert.ToInt64(productId.Rows[0]["PRODUCT_ID"]);
        //        }

        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public long AddProductCategory(long tenantId,Model.Product.ProductCategoryPayload productCategory)
        {
            try
            {
                DataTable dtProductCategory = new DataTable();
                dtProductCategory.Columns.Add("PRODUCTID", typeof(long));
                dtProductCategory.Columns.Add("CATEGORYID", typeof(long));
                foreach (var CategoryId in productCategory.CategoryId)
                {
                    DataRow drdtProductCategoryRow = dtProductCategory.NewRow();
                    drdtProductCategoryRow["PRODUCTID"] = productCategory.ProductId;
                    drdtProductCategoryRow["CATEGORYID"] = CategoryId;
                    dtProductCategory.Rows.Add(drdtProductCategoryRow);
                }

                //insert the invoice details
                var ProductCategoryId = _dataAccess.ExecuteScalar(Model.Constant.Constant.StoredProcedures.SP_PRODUCT_CATEGORY_MAPPING,
                dtProductCategory);

                return Convert.ToInt64(ProductCategoryId);
            }
                catch (Exception ex)
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
        //         var categoryId = _dataAccess.ExecuteDataTable(Model.Constant.Constant.StoredProcedures.SP_ADD_CATEGORY,
        //                             tenantId,
        //                             catrtegoryPayload.Category,
        //                             catrtegoryPayload.SubCategory,
        //                             catrtegoryPayload.SubMenu,
        //                             catrtegoryPayload.Link,
        //                             catrtegoryPayload.SubMenu,
        //                             catrtegoryPayload.OrderBy,
        //                             catrtegoryPayload.Active);

        //         if (categoryId.Rows.Count > 0)
        //         {
        //             return Convert.ToInt64(categoryId.Rows[0]["CATEGORY_ID"]);
        //         }

        //         return 0;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw;
        //     }
        // }
        
        /// <summary>
        /// UpsertCart
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public DataTable UpsertCart(CartPayload payload)
        {
            try
            {
                return _dataAccess.ExecuteDataTable(Model.Constant.Constant.StoredProcedures.SP_UPSERT_CART,
                    payload.UserId,
                    payload.ProductId,
                    payload.Quantity);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// UpsertWishList
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public DataTable UpsertWishList(WishListPayload payload)
        {
            try
            {
                return _dataAccess.ExecuteDataTable(Model.Constant.Constant.StoredProcedures.SP_UPSERT_WISHLIST,
                    payload.UserId,
                    payload.ProductId,
                    payload.Quantity);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.ProductCart.SpProductCart> GetUserCart(string spName, long userId)
        {
            try
            {
                //Executing query
                List<Model.ProductCart.SpProductCart> spUserMasterLists = _dataAccess.ExecuteGenericList<Model.ProductCart.SpProductCart>(spName,
                    userId);

                return spUserMasterLists;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public DataTable ValidateVendor(long tenantId, long locationId)
        //{
        //    try
        //    {
        //        return _dataAccess.ExecuteDataTable(Model.Constant.Constant.StoredProcedures.SA_REALTIMEOCR_VALIDATE_VENDOR, tenantId, locationId);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //internal async Task<List<ProductMaster>> GetProductMaster(string tenantId, PorductFilter filter)
        //{
        //    try
        //    {
        //        //Local variable
        //        List<Model.Product.ProductMaster> productMasters = new List<Model.Product.ProductMaster>();

        //        #region Query Builder

        //        if (filter.IncludeReviews && filter.IncludeImages)
        //        {
        //            productMasters = await this.DbContext.productMasters.Include("ProductImages").
        //                                                                    Include("ProductReviews")
        //                                                                    .Where(x => (!filter.IncludeInactive ? x.Active.Equals(!filter.IncludeInactive) : filter.IncludeInactive))
        //                                                                    .AsNoTracking().ToListAsync();
        //        }
        //        else if (filter.IncludeImages)
        //        {
        //            productMasters = await this.DbContext.productMasters.Include("ProductImages").Where(x => (!filter.IncludeInactive ? x.Active.Equals(!filter.IncludeInactive) : filter.IncludeInactive))
        //                                                                    .AsNoTracking().ToListAsync();
        //        }
        //        else if (filter.IncludeReviews)
        //        {
        //            productMasters = await this.DbContext.productMasters.Include("ProductReviews").Where(x => (!filter.IncludeInactive ? x.Active.Equals(!filter.IncludeInactive) : filter.IncludeInactive))
        //                                                                    .AsNoTracking().ToListAsync();
        //        }
        //        else
        //        {
        //            productMasters = await this.DbContext.productMasters.Where(x => (!filter.IncludeInactive ? x.Active.Equals(!filter.IncludeInactive) : filter.IncludeInactive))
        //                                                .AsNoTracking().ToListAsync();
        //        }

        //        // Null check
        //        if (productMasters == null || productMasters.Count == 0)
        //        {
        //            this.Logger.LogWarning($"product not available for tenantId : {tenantId}");
        //            throw new KeyNotFoundException($"product not available f");
        //        }

        //        return productMasters;
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    #endregion
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        internal List<ProductCategory> GetCategory(string tenantId)
        {
            try
            {
                // method 1
                List<Model.Product.ProductCategory> productCategories = this.DbContext.productCategories
                .Where(x => x.TenantId.Equals(tenantId)).ToList();


                SqlParameter Tenant = new SqlParameter
                {
                    ParameterName = "@TenantId",
                    SqlDbType = SqlDbType.BigInt,
                    Value = Convert.ToInt64(tenantId)
                };

                // method 2
                string spName = "XC_GET_CATEGORY_TEST";
                List<Model.Product.ProductCategory> productCategories1 = this.DbContext.productCategories.FromSqlRaw($"exec {spName} @TenantId", Tenant).AsNoTracking().ToList();

                return productCategories1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetProductVerificationDataToEdit(string tenantId, string productId, string spname, string connectionstring)
        {
            try
            {
                //Logger 
                this.Logger.LogInformation($"Calling the GetProductVerificationDataToEdit for tenant : {tenantId}, product : {productId}");

                this.Logger.LogInformation($"Executing {spname} to fetch product verification item details for tenant : {tenantId}, product : {productId}");

                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand sqlComm = new SqlCommand(spname, conn);
                    sqlComm.Parameters.AddWithValue("@PRODUCTID", Convert.ToInt64(productId));
                    sqlComm.Parameters.AddWithValue("@TENANT_ID", Convert.ToInt64(tenantId));

                    sqlComm.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;

                    da.Fill(ds);
                }
                //return reacord
                return ds;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"error while calling the GetProductVerificationDataToEdit :{ex.Message}");
                this.Logger.LogError($"Inner exception: {ex.InnerException}");
                this.Logger.LogError($"Stack trace{ex.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// // method 3
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <param name="orderBy"></param>
        /// <param name="order"></param>
        /// <param name="spName"></param>
        /// <returns></returns>
        public List<Model.Product.SpProductMasterList> GetProductList(string spName, string tenantId, Model.Product.ProductPayload payload, string orderBy, string order)
        {
            try
            {
                DataTable productIdList = new DataTable();
                productIdList.Columns.Add("VALUE", typeof(Int64));

                DataRow dr;
                payload.ProductId.ForEach(x =>
                {
                    dr = productIdList.NewRow();
                    dr["VALUE"] = x;
                    productIdList.Rows.Add(dr);
                });

                //Executing query
                List<Model.Product.SpProductMasterList> spProductMasterLists = _dataAccess.ExecuteGenericList<Model.Product.SpProductMasterList>(spName,
                    Convert.ToInt64(tenantId),
                    Convert.ToInt64(payload.RoleId),
                    Convert.ToInt32(payload.Page),
                    Convert.ToInt32(payload.PageSize),
                    orderBy, order, payload.Search ?? string.Empty,
                    payload.Category,
                    productIdList
                    );


                return spProductMasterLists;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

                

        //public Models.Group GetProductGroupById(string tenantId, string groupId, string SpName)
        //{
        //    try
        //    {
        //        Models.Group group = _dataAccess.ExecuteGenericList<Models.Group>(SpName, tenantId, groupId).AsEnumerable().FirstOrDefault();
        //        SetProductGroupAggregate(new List<Models.Group> { group });

        //        return group;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="spName"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        internal List<Model.Product.ProductMaster> GetProductDtailsUsingSp(string tenantId, string spName, Model.Product.Search search)
        {
            try
            {
                #region Vendor
                //vendor
                DataTable subcategoryCollection = new DataTable();
                subcategoryCollection.Columns.Add("VALUE", typeof(Int64));

                if (search.SubCategory != null && search.SubCategory.Count() > 0)
                {
                    DataRow dr;
                    search.SubCategory.ForEach(x =>
                    {
                        dr = subcategoryCollection.NewRow();
                        dr["VALUE"] = Convert.ToInt64(x);
                        subcategoryCollection.Rows.Add(dr);
                    });
                }
                #endregion

                #region Category
                //Category
                DataTable categoryCollection = new DataTable();
                categoryCollection.Columns.Add("VALUE", typeof(Int64));

                if (search.Category != null && search.Category.Count() > 0)
                {
                    DataRow dr;
                    search.Category.ForEach(x =>
                    {
                        dr = categoryCollection.NewRow();
                        dr["VALUE"] = Convert.ToInt64(x);
                        categoryCollection.Rows.Add(dr);
                    });
                }
                #endregion

                //Executing query
                List<Model.Product.ProductMaster> productDetails = _dataAccess.ExecuteGenericList<Model.Product.ProductMaster>(spName, tenantId, search.isActive, categoryCollection, subcategoryCollection);

                //return product details
                return productDetails;

            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Inner exception : {ex.InnerException}");
                this.Logger.LogError($"Stack trace : {ex.StackTrace}");
                throw ex;
            }
        }

        public async Task<Model.Product.ProductMaster> GetPartnerEventDetails(string RestaurantGuid, string LocationGuid, string storeProcedureName)
        {
            SqlParameter RestaurantGuidParam = Utility.PrepareParametersForStoreProcedure("@RESTAURANTGUID", SqlDbType.VarChar, string.Empty, RestaurantGuid);
            SqlParameter LocationGuidParam = Utility.PrepareParametersForStoreProcedure("@LOCATIONGUID", SqlDbType.VarChar, string.Empty, LocationGuid);
            Model.Product.ProductMaster partnerEventDetails = this.DbContext.productMasters.FromSqlRaw($"exec {storeProcedureName} @RESTAURANTGUID, @LOCATIONGUID", RestaurantGuidParam, LocationGuidParam).AsNoTracking().AsEnumerable().FirstOrDefault();

            return partnerEventDetails;
        }

        //public List<Model.Product.SpProductMasterList> GetProductMaster(string tenantId, 
        //    Model.Product.ProductPayload payload, string spName)
        //{
        //    try
        //    {
        //        DataTable category = new DataTable();
        //        category.Columns.Add("VALUE", typeof(Int64));

        //        DataRow dr;
        //        payload.Category.ForEach(x =>
        //        {
        //            dr = category.NewRow();
        //            dr["VALUE"] = x;
        //            category.Rows.Add(dr);
        //        });

        //        //Executing query
        //        List<Model.Product.ProductMaster> productMasters = _dataAccess.ExecuteGenericList<Model.Product.ProductMaster>(spName, 
        //            Convert.ToInt64(tenantId), 
        //            category);

        //        return productMasters;

        //    }
        //    catch (Exception ex)
        //    {
        //        //logger
        //        throw;
        //    }
        //}


        // only sample
        public async Task<List<Model.Product.ProductMaster>> GetStandardUoms(string spname)
        {
            try
            {
                //get standarduom list
                List<Model.Product.ProductMaster> uomstandard = _dataAccess.ExecuteGenericList<Model.Product.ProductMaster>(spname);

                return uomstandard;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Asynchronously retrieves an image by its ID.
        /// </summary>
        /// <param name="Id">The ID of the image to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a DataTable with the image data.</returns>
        public async Task<DataTable> GetImageAsync(long Id)
        {
            try
            {
                return await Task.Run(() => _dataAccess.ExecuteDataTable("SP_GET_IMAGE_BY_ID", Id));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ProductImageName"></param>
        /// <param name="ContentType"></param>
        /// <param name="ImageData"></param>
        /// <param name="ThumbnailData"></param>
        /// <param name="FileSize"></param>
        /// <returns></returns>
        public long AddImages(long ProductId, string ProductImageName, string ContentType, byte[] ImageData, byte[] ThumbnailData, int FileSize)
        {
            try
            {
                var cmd = _dataAccessCmd.ExecuteNonQueryCMD(
                    Model.Constant.Constant.StoredProcedures.SP_ADD_IMAGES,
                    ProductId,
                    ProductImageName,
                    ContentType,
                    ImageData,
                    ThumbnailData,
                    FileSize);

                // Get the return value from the stored procedure
                return Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfigValueByKey(string key)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CONFIG_KEY", key }
                    // { "TenantId", tenantId },
                    // { "AuditInfo", auditInfo },
                    // { "UserId", userId },
                    // { "BrowserName", payload.BrowserName },
                    // { "BrowserVersion", payload.BrowserVersion }
                };
                var slaValue = _dataAccess.ExecuteScalarSQL(Constant.StoredProcedures.SP_GET_VALUE_BY_KEY, parameters);
                
                return slaValue.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="userId">User ID performing the deletion</param>
        /// <returns>Task</returns>
        public async Task DeleteProduct(long tenantId, long productId, long userId)
        {
            try
            {
                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_DELETE_PRODUCT,
                    productId,
                    tenantId,
                    userId
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Product not found or does not belong to this tenant");
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                var result = _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_ALL_CATEGORIES,
                    tenantId ?? (object)DBNull.Value
                );

                var categories = new List<CategoryListItem>();

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        categories.Add(new CategoryListItem
                        {
                            CategoryId = Convert.ToInt64(row["CategoryId"]),
                            Category = row["Category"].ToString(),
                            Active = Convert.ToBoolean(row["Active"]),
                            SubMenu = Convert.ToBoolean(row["SubMenu"]),
                            Created = row["Created"] != DBNull.Value ? Convert.ToDateTime(row["Created"]) : (DateTime?)null,
                            Modified = row["Modified"] != DBNull.Value ? Convert.ToDateTime(row["Modified"]) : (DateTime?)null,
                            OrderBy = row["OrderBy"] != DBNull.Value ? Convert.ToInt32(row["OrderBy"]) : (int?)null,
                            Description = row["Description"]?.ToString(),
                            Icon = row["Icon"]?.ToString(),
                            ParentCategoryId = row["ParentCategoryId"] != DBNull.Value ? Convert.ToInt64(row["ParentCategoryId"]) : (long?)null,
                            TenantId = Convert.ToInt64(row["TenantId"])
                        });
                    }
                }

                return categories;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>   
        /// Add a new category
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Category details</param>
        /// <param name="userId">User ID creating the category</param>
        /// <returns>Newly created category ID</returns>
        public async Task<long> AddCategory(long tenantId, AddCategoryRequest request, long userId)
        {
            try
            {
                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_ADD_CATEGORY,
                    tenantId,
                    request.CategoryName,
                    request.Description ?? (object)DBNull.Value,
                    request.Active,
                    request.ParentCategoryId ?? (object)DBNull.Value,
                    request.OrderBy,
                    request.Icon ?? (object)DBNull.Value,
                    request.HasSubMenu,
                    request.Link ?? (object)DBNull.Value,
                    userId,
                    request.MenuId ?? (object)DBNull.Value
                ));

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt64(result.Tables[0].Rows[0]["CategoryId"]);
                }

                throw new Exception("Failed to get category ID after insertion");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="request">Category details</param>
        /// <param name="userId">User ID performing the update</param>
        /// <returns>Task</returns>
        public async Task UpdateCategory(long tenantId, UpdateCategoryRequest request, long userId)
        {
            try
            {
                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_UPDATE_CATEGORY,
                    request.CategoryId,
                    tenantId,
                    request.CategoryName,
                    request.Description ?? (object)DBNull.Value,
                    request.Active,
                    request.ParentCategoryId ?? (object)DBNull.Value,
                    request.OrderBy,
                    request.Icon ?? (object)DBNull.Value,
                    request.HasSubMenu,
                    request.Link ?? (object)DBNull.Value,
                    userId
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Category not found or does not belong to this tenant");
                }
            }
            catch (Exception ex)
            {
                throw;
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
                var result = _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_MENU_MASTER,
                    tenantId ?? (object)DBNull.Value
                );

                var response = new MenuMasterResponse();
                var menuDictionary = new Dictionary<long, MenuMasterItem>();

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var menuId = Convert.ToInt64(row["MenuId"]);
                        
                        // Create or get menu item
                        if (!menuDictionary.ContainsKey(menuId))
                        {
                            menuDictionary[menuId] = new MenuMasterItem
                            {
                                MenuId = menuId,
                                MenuName = row["MenuName"].ToString(),
                                OrderBy = Convert.ToInt32(row["OrderBy"]),
                                Active = Convert.ToBoolean(row["Active"]),
                                Image = row["Image"]?.ToString() ?? "",
                                SubMenu = Convert.ToBoolean(row["SubMenu"]),
                                TenantId = Convert.ToInt64(row["TenantId"]),
                                Created = row["Created"] != DBNull.Value ? Convert.ToDateTime(row["Created"]) : (DateTime?)null,
                                Modified = row["Modified"] != DBNull.Value ? Convert.ToDateTime(row["Modified"]) : (DateTime?)null,
                                Category = new List<MenuCategoryItem>()
                            };
                        }

                        // Add category if it exists
                        if (row["CategoryId"] != DBNull.Value)
                        {
                            var categoryId = Convert.ToInt64(row["CategoryId"]);
                            var existingCategory = menuDictionary[menuId].Category
                                .FirstOrDefault(c => c.CategoryId == categoryId);

                            if (existingCategory == null)
                            {
                                menuDictionary[menuId].Category.Add(new MenuCategoryItem
                                {
                                    CategoryId = categoryId,
                                    Category = row["Category"].ToString(),
                                    Active = Convert.ToBoolean(row["CategoryActive"]),
                                    OrderBy = row["CategoryOrderBy"] != DBNull.Value ? Convert.ToInt32(row["CategoryOrderBy"]) : (int?)null,
                                    Icon = row["CategoryIcon"]?.ToString(),
                                    Description = row["CategoryDescription"]?.ToString()
                                });
                            }
                        }
                    }

                    // Convert dictionary to list and sort
                    response.MenuMaster = menuDictionary.Values
                        .OrderBy(m => m.OrderBy)
                        .ThenBy(m => m.MenuName)
                        .ToList();

                    // Sort categories within each menu
                    foreach (var menu in response.MenuMaster)
                    {
                        menu.Category = menu.Category
                            .OrderBy(c => c.OrderBy ?? 0)
                            .ThenBy(c => c.Category)
                            .ToList();
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw;
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
                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_UPDATE_PRODUCT,
                    request.ProductId,
                    tenantId,
                    request.ProductName,
                    request.ProductDescription,
                    request.ProductCode,
                    request.FullDescription,
                    request.Specification,
                    request.Story,
                    request.PackQuantity,
                    request.Quantity,
                    request.Total,
                    request.Price,
                    request.Category,
                    request.Rating,
                    request.Active,
                    request.Trending,
                    request.UserBuyCount,
                    request.Return,
                    request.BestSeller,
                    request.DeliveryDate,
                    request.Offer,
                    request.OrderBy,
                    request.UserId,
                    request.UserId // ModifiedBy
                ));

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt64(result.Tables[0].Rows[0]["ProductId"]);
                }

                throw new KeyNotFoundException("Product not found or update failed");
            }
            catch (Exception ex)
            {
                throw;
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
                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_ADD_PRODUCT,
                    tenantId,
                    request.ProductName,
                    request.ProductDescription,
                    request.ProductCode,
                    request.FullDescription,
                    request.Specification,
                    request.Story,
                    request.PackQuantity,
                    request.Quantity,
                    request.Total,
                    request.Price,
                    request.Category,
                    request.Rating,
                    request.Active,
                    request.Trending,
                    request.UserBuyCount,
                    request.Return,
                    request.BestSeller,
                    request.DeliveryDate,
                    request.Offer,
                    request.OrderBy,
                    request.UserId,
                    request.UserId // CreatedBy
                ));

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt64(result.Tables[0].Rows[0]["ProductId"]);
                }

                throw new Exception("Failed to get product ID after insertion");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get product details by ID
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>DataSet containing product details and images</returns>
        public async Task<DataSet> GetProductById(long productId)
        {
            try
            {
                return await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_PRODUCT_BY_ID,
                    productId
                ));
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
        public long DeleteImages(long id)
        {
            try
            {
                var cmd = _dataAccessCmd.ExecuteNonQueryCMD(Model.Constant.Constant.StoredProcedures.SP_DELETE_IMAGES, id);
                return Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public void InsertCategory(string tenantId, Model.Response.Category category)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            using (var command = new SqlCommand("sp_InsertCategory", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@TenantId", tenantId);
        //                command.Parameters.AddWithValue("@CategoryId", category.CategoryId);
        //                command.Parameters.AddWithValue("@Name", category.Name);
        //                command.Parameters.AddWithValue("@Link", category.link);

        //                connection.Open();
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while inserting the category", ex);
        //    }
        //}

        /// <summary>
        /// Search 
        /// 
        /// with advanced filtering and pagination
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="payload">Search parameters</param>
        /// <param name="offset">Pagination offset</param>
        /// <returns>DataSet containing products and total count</returns>
        public DataSet SearchProducts(string tenantId, ProductSearchPayload payload, int offset)
        {
            try
            {
                // Execute stored procedure for product search
                DataSet result = _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_SEARCH_PRODUCTS,
                    Convert.ToInt64(tenantId),
                    payload.Page,
                    payload.Limit,
                    offset,
                    payload.Search ?? string.Empty,
                    payload.Category,
                    payload.MinPrice,
                    payload.MaxPrice,
                    payload.Rating,
                    payload.InStock,
                    payload.BestSeller,
                    payload.HasOffer,
                    payload.SortBy ?? "created",
                    payload.SortOrder ?? "desc"
                );

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get user's shopping cart with full product details
        /// </summary>
        /// <param name="request">Cart request with user details</param>
        /// <returns>Complete cart information</returns>
        public async Task<Model.User.CartResponse> GetUserCart(Model.ProductCart.GetCartRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Get cart for user {request.UserId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_GET_USER_CART,
                    request.UserId,
                    request.TenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }

                var cartResponse = new Model.User.CartResponse();
                var cartItems = new Dictionary<long, Model.User.CartItem>();

                // Process cart items (first result set)
                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var cartId = Convert.ToInt64(row["CartId"]);
                        
                        if (!cartItems.ContainsKey(cartId))
                        {
                            var cartItem = new Model.User.CartItem
                            {
                                Id = $"cart_item_{cartId}",
                                CartId = cartId,
                                UserId = Convert.ToInt64(row["UserId"]),
                                TenantId = Convert.ToInt64(row["TenantId"]),
                                Quantity = Convert.ToInt32(row["Quantity"]),
                                AddedDate = Convert.ToDateTime(row["AddedDate"]),
                                UpdatedDate = row["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedDate"]) : null,
                                SessionId = row["SessionId"]?.ToString() ?? "",
                                ItemTotal = Convert.ToDecimal(row["ItemTotal"]),
                                IsAvailable = Convert.ToBoolean(row["IsAvailable"]),
                                Product = new Model.User.CartProductDetails
                                {
                                    ProductId = Convert.ToInt64(row["ProductId"]),
                                    ProductName = row["ProductName"]?.ToString() ?? "",
                                    ProductDescription = row["ProductDescription"]?.ToString() ?? "",
                                    ProductCode = row["ProductCode"]?.ToString() ?? "",
                                    FullDescription = row["FullDescription"]?.ToString() ?? "",
                                    Specification = row["Specification"]?.ToString() ?? "",
                                    Story = row["Story"]?.ToString() ?? "",
                                    PackQuantity = row["PackQuantity"] != DBNull.Value ? Convert.ToInt32(row["PackQuantity"]) : 0,
                                    AvailableQuantity = Convert.ToInt32(row["ProductAvailableQuantity"]),
                                    Price = Convert.ToDecimal(row["Price"]),
                                    Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                                    Active = Convert.ToBoolean(row["ProductActive"]),
                                    Trending = row["Trending"] != DBNull.Value ? Convert.ToBoolean(row["Trending"]) : false,
                                    UserBuyCount = row["UserBuyCount"] != DBNull.Value ? Convert.ToInt32(row["UserBuyCount"]) : 0,
                                    ReturnPolicy = row["ReturnPolicy"] != DBNull.Value ? Convert.ToInt32(row["ReturnPolicy"]) : 0,
                                    Created = Convert.ToDateTime(row["ProductCreated"]),
                                    Modified = row["ProductModified"] != DBNull.Value ? Convert.ToDateTime(row["ProductModified"]) : null,
                                    InStock = row["InStock"] != DBNull.Value ? Convert.ToBoolean(row["InStock"]) : false,
                                    BestSeller = row["BestSeller"] != DBNull.Value ? Convert.ToBoolean(row["BestSeller"]) : false,
                                    DeliveryDate = row["DeliveryDate"] != DBNull.Value ? Convert.ToInt32(row["DeliveryDate"]) : 0,
                                    Offer = row["Offer"]?.ToString() ?? "",
                                    Overview = row["Overview"]?.ToString() ?? "",
                                    LongDescription = row["LongDescription"]?.ToString() ?? "",
                                    Category = new Model.User.CartProductCategory
                                    {
                                        CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt64(row["CategoryId"]) : 0,
                                        CategoryName = row["CategoryName"]?.ToString() ?? "",
                                        Active = row["CategoryActive"] != DBNull.Value ? Convert.ToBoolean(row["CategoryActive"]) : false,
                                        Description = row["CategoryDescription"]?.ToString() ?? "",
                                        Icon = row["CategoryIcon"]?.ToString() ?? "",
                                        SubMenu = row["CategorySubMenu"] != DBNull.Value ? Convert.ToBoolean(row["CategorySubMenu"]) : false
                                    },
                                    Images = new List<Model.User.CartProductImage>()
                                }
                            };

                            cartItems[cartId] = cartItem;
                        }

                        // Add product image if exists and images are requested
                        if (request.IncludeImages && row["ImageId"] != DBNull.Value)
                        {
                            var imageId = Convert.ToInt64(row["ImageId"]);
                            var existingImage = cartItems[cartId].Product.Images.FirstOrDefault(img => img.ImageId == imageId);
                            
                            if (existingImage == null)
                            {
                                cartItems[cartId].Product.Images.Add(new Model.User.CartProductImage
                                {
                                    ImageId = imageId,
                                    Poster = row["ImageUrl"] != DBNull.Value ? row["ImageUrl"].ToString() : "",
                                    Main = row["IsMainImage"] != DBNull.Value ? Convert.ToBoolean(row["IsMainImage"]) : false,
                                    Active = row["ImageActive"] != DBNull.Value ? Convert.ToBoolean(row["ImageActive"]) : false,
                                    OrderBy = row["ImageOrderBy"] != DBNull.Value ? Convert.ToInt32(row["ImageOrderBy"]) : 0
                                });
                            }
                        }
                    }

                    cartResponse.Items = cartItems.Values.OrderByDescending(item => item.AddedDate).ToList();
                }

                // Process cart summary (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var summaryRow = result.Tables[1].Rows[0];
                    cartResponse.Summary = new Model.User.CartSummary
                    {
                        TotalItems = summaryRow["TotalItems"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalItems"]) : 0,
                        TotalQuantity = summaryRow["TotalQuantity"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalQuantity"]) : 0,
                        TotalAmount = summaryRow["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(summaryRow["TotalAmount"]) : 0m,
                        AvailableItemsTotal = summaryRow["AvailableItemsTotal"] != DBNull.Value ? Convert.ToDecimal(summaryRow["AvailableItemsTotal"]) : 0m,
                        UnavailableItems = summaryRow["UnavailableItems"] != DBNull.Value ? Convert.ToInt32(summaryRow["UnavailableItems"]) : 0
                    };
                }

                // Process recommended products (third result set) if requested
                if (request.IncludeRecommendations && result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    cartResponse.RecommendedProducts = new List<Model.User.RecommendedProduct>();
                    foreach (DataRow row in result.Tables[2].Rows)
                    {
                        cartResponse.RecommendedProducts.Add(new Model.User.RecommendedProduct
                        {
                            ProductId = Convert.ToInt64(row["ProductId"]),
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            Price = Convert.ToDecimal(row["Price"]),
                            Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                            BestSeller = row["BestSeller"] != DBNull.Value ? Convert.ToBoolean(row["BestSeller"]) : false,
                            Offer = row["Offer"]?.ToString() ?? "",
                            ImageUrl = row["ImageUrl"]?.ToString() ?? ""
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Cart retrieval successful for user {request.UserId} - {cartResponse.Items.Count} items");

                return cartResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Cart retrieval error for user {request.UserId}: {ex.Message}");
                
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException(ex.Message);
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Get wishlist items for user {userId}, tenant {tenantId}");

                // Prepare parameters - ensure tenantId is properly handled
                var tenantIdParam = tenantId.HasValue ? (object)tenantId.Value : (object)DBNull.Value;
                this.Logger.LogInformation($"Repository: Calling SP_GET_USER_WISHLIST_ITEMS with userId={userId}, tenantId={tenantIdParam}");

                DataSet result;
                try
                {
                    result = await Task.Run(() => _dataAccess.ExecuteDataset(
                        Model.Constant.Constant.StoredProcedures.SP_GET_USER_WISHLIST_ITEMS,
                        userId,
                        tenantIdParam
                    ));
                }
                catch (System.Net.Sockets.SocketException socketEx)
                {
                    this.Logger.LogError($"Repository: SocketException when calling stored procedure: {socketEx.Message}. This may indicate a database connection issue or the stored procedure doesn't exist.", socketEx);
                    throw new InvalidOperationException($"Database connection error or stored procedure not found. Please ensure SP_GET_USER_WISHLIST_ITEMS exists in the database. Original error: {socketEx.Message}", socketEx);
                }
                catch (Exception dbEx)
                {
                    this.Logger.LogError($"Repository: Database error when calling stored procedure: {dbEx.Message}", dbEx);
                    throw;
                }

                var response = new Model.WishList.WishlistItemsResponse
                {
                    Items = new List<Model.WishList.WishlistItemWithProduct>(),
                    Page = 1,
                    PageSize = 100
                };

                if (result == null || result.Tables.Count == 0)
                {
                    response.TotalCount = 0;
                    response.TotalPages = 0;
                    response.HasNext = false;
                    response.HasPrevious = false;
                    return response;
                }

                // Process wishlist items (first result set)
                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var wishlistItem = new Model.WishList.WishlistItemWithProduct
                        {
                            WishlistItemId = row["WishListId"] != DBNull.Value ? Convert.ToInt64(row["WishListId"]) : 0,
                            WishlistId = row["WishListId"] != DBNull.Value ? Convert.ToInt64(row["WishListId"]) : 0,
                            ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt64(row["ProductId"]) : 0,
                            Quantity = 1,
                            Priority = row["Priority"] != DBNull.Value ? Convert.ToInt32(row["Priority"]) : 3,
                            Notes = row["Notes"]?.ToString() ?? "",
                            NotifyOnSale = false,
                            NotifyOnRestock = false,
                            NotifyOnPriceDrop = false,
                            AddedAt = row["AddedAt"] != DBNull.Value ? Convert.ToDateTime(row["AddedAt"]) : DateTime.UtcNow,
                            UpdatedAt = row["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedAt"]) : DateTime.UtcNow,
                            Product = new Model.WishList.WishlistProduct
                            {
                                ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt64(row["ProductId"]) : 0,
                                ProductName = row["ProductName"]?.ToString() ?? "",
                                ProductDescription = row["ProductDescription"]?.ToString() ?? "",
                                CurrentPrice = row["CurrentPrice"] != DBNull.Value ? Convert.ToDecimal(row["CurrentPrice"]) : 0,
                                OriginalPrice = row["OriginalPrice"] != DBNull.Value ? Convert.ToDecimal(row["OriginalPrice"]) : (decimal?)null,
                                InStock = row["InStock"] != DBNull.Value ? Convert.ToBoolean(row["InStock"]) : false,
                                StockQuantity = row["ProductAvailableQuantity"] != DBNull.Value ? Convert.ToInt32(row["ProductAvailableQuantity"]) : 0,
                                Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                                TotalReviews = row["TotalReviews"] != DBNull.Value ? Convert.ToInt32(row["TotalReviews"]) : 0,
                                DiscountPercentage = row["DiscountPercentage"] != DBNull.Value ? Convert.ToDecimal(row["DiscountPercentage"]) : 0,
                                OfferText = row["Offer"]?.ToString() ?? "",
                                ProductImage = row["ProductImage"]?.ToString() ?? "",
                                CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt64(row["CategoryId"]) : 0,
                                CategoryName = row["CategoryName"]?.ToString() ?? "",
                                IsActive = row["ProductActive"] != DBNull.Value ? Convert.ToBoolean(row["ProductActive"]) : false,
                                UpdatedAt = row["ProductModified"] != DBNull.Value ? Convert.ToDateTime(row["ProductModified"]) : DateTime.UtcNow
                            },
                            Alerts = new Model.WishList.WishlistAlerts
                            {
                                OnSale = row["OnSale"] != DBNull.Value ? Convert.ToBoolean(row["OnSale"]) : false,
                                BackInStock = false,
                                PriceAlertTriggered = false,
                                PriceDropped = false
                            }
                        };

                        response.Items.Add(wishlistItem);
                    }
                }

                // Get summary (second result set if available)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var summaryRow = result.Tables[1].Rows[0];
                    response.TotalCount = summaryRow["TotalItems"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalItems"]) : response.Items.Count;
                }
                else
                {
                    response.TotalCount = response.Items.Count;
                }

                // Calculate pagination
                response.TotalPages = (int)Math.Ceiling((double)response.TotalCount / response.PageSize);
                response.HasNext = response.Page < response.TotalPages;
                response.HasPrevious = response.Page > 1;

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting wishlist items: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Add item to cart
        /// </summary>  
        /// <param name="request">Add to cart request</param>
        /// <returns>Cart item details and summary</returns>
        public async Task<Model.ProductCart.AddToCartResponse> AddItemToCart(long tenantId, Model.ProductCart.AddToCartRequest request)
        {
            const int maxRetries = 3;
            const int baseDelayMs = 100; // Base delay in milliseconds
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    this.Logger.LogInformation($"Repository: Add to cart for user {request.UserId}, product {request.ProductId} (Attempt {retryCount + 1})");

                    var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                        Model.Constant.Constant.StoredProcedures.SP_ADD_ITEM_TO_CART,
                        request.UserId,
                        request.ProductId,
                        request.Quantity,
                        tenantId.ToString() ?? (object)DBNull.Value,
                        request.SessionId ?? (object)DBNull.Value,
                        request.IpAddress ?? (object)DBNull.Value,
                        request.UserAgent ?? (object)DBNull.Value
                    ));

                    if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    {
                        throw new KeyNotFoundException("Failed to add item to cart - user or product not found");
                    }

                    // Process cart item result (first result set)
                    var cartItemRow = result.Tables[0].Rows[0];
                    var cartResponse = new Model.ProductCart.AddToCartResponse
                    {
                        CartId = cartItemRow["CartId"] != DBNull.Value ? Convert.ToInt64(cartItemRow["CartId"]) : 0,
                        UserId = cartItemRow["UserId"] != DBNull.Value ? Convert.ToInt64(cartItemRow["UserId"]) : 0,
                        ProductId = cartItemRow["ProductId"] != DBNull.Value ? Convert.ToInt64(cartItemRow["ProductId"]) : 0,
                        ProductName = cartItemRow["ProductName"]?.ToString() ?? "",
                        Quantity = cartItemRow["Quantity"] != DBNull.Value ? Convert.ToInt32(cartItemRow["Quantity"]) : 0,
                        Price = cartItemRow["Price"] != DBNull.Value ? Convert.ToDecimal(cartItemRow["Price"]) : 0m,
                        ItemTotal = cartItemRow["ItemTotal"] != DBNull.Value ? Convert.ToDecimal(cartItemRow["ItemTotal"]) : 0m,
                        Message = cartItemRow["Message"]?.ToString() ?? "Product added to cart successfully",
                        UpdatedDate = cartItemRow.Table.Columns.Contains("UpdatedDate") 
                            ? (cartItemRow["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(cartItemRow["UpdatedDate"]) : DateTime.MinValue)
                            : (cartItemRow["AddedDate"] != DBNull.Value ? Convert.ToDateTime(cartItemRow["AddedDate"]) : DateTime.MinValue)
                    };

                    // Process cart summary (second result set)
                    if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                    {
                        var summaryRow = result.Tables[1].Rows[0];
                        cartResponse.Summary = new Model.ProductCart.CartSummaryInfo
                        {
                            TotalUniqueItems = summaryRow["TotalUniqueItems"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalUniqueItems"]) : 0,
                            TotalQuantity = summaryRow["TotalQuantity"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalQuantity"]) : 0,
                            TotalAmount = summaryRow["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(summaryRow["TotalAmount"]) : 0m
                        };
                    }

                    this.Logger.LogInformation($"Repository: Add to cart successful for user {request.UserId}, product {request.ProductId}");

                    return cartResponse;
                }
                catch (Exception ex)
                {
                    // Check if this is a deadlock error (SQL Server error 1205)
                    bool isDeadlock = false;
                    
                    // Check for SqlException with error number 1205 (deadlock victim)
                    if (ex is SqlException sqlEx && sqlEx.Number == 1205)
                    {
                        isDeadlock = true;
                    }
                    // Also check message content as fallback
                    else if (ex.Message.Contains("deadlocked") || 
                             ex.Message.Contains("deadlock victim") ||
                             (ex.InnerException != null && (
                                 ex.InnerException.Message.Contains("deadlocked") ||
                                 ex.InnerException.Message.Contains("deadlock victim"))))
                    {
                        isDeadlock = true;
                    }

                    if (isDeadlock && retryCount < maxRetries - 1)
                    {
                        retryCount++;
                        // Exponential backoff: 100ms, 200ms, 400ms
                        int delayMs = baseDelayMs * (int)Math.Pow(2, retryCount - 1);
                        
                        this.Logger.LogWarning($"Repository: Deadlock detected on add to cart for user {request.UserId}, product {request.ProductId}. Retrying in {delayMs}ms (Attempt {retryCount + 1}/{maxRetries})");
                        
                        await Task.Delay(delayMs);
                        continue; // Retry the operation
                    }

                    this.Logger.LogError($"Repository: Add to cart error for user {request.UserId}, product {request.ProductId}: {ex.Message}");
                    
                    // Check for specific error messages from stored procedure
                    if (ex.Message.Contains("User not found or inactive"))
                    {
                        throw new KeyNotFoundException("User not found or inactive");
                    }
                    else if (ex.Message.Contains("Product not found or inactive"))
                    {
                        throw new KeyNotFoundException("Product not found or inactive");
                    }
                    else if (ex.Message.Contains("Insufficient stock"))
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                    else if (ex.Message.Contains("Quantity must be greater than 0"))
                    {
                        throw new ArgumentException("Quantity must be greater than 0");
                    }
                    else if (isDeadlock)
                    {
                        throw new InvalidOperationException("Unable to add item to cart due to database deadlock. Please try again.", ex);
                    }
                    
                    throw;
                }
            }

            // This should never be reached, but just in case
            throw new InvalidOperationException("Failed to add item to cart after multiple retry attempts.");
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
                this.Logger.LogInformation($"Repository: Remove from cart for user {request.UserId}, product {request.ProductId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_REMOVE_ITEM_FROM_CART,
                    request.UserId,
                    request.ProductId,
                    tenantId.ToString() ?? (object)DBNull.Value,
                    request.RemoveCompletely,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Product not found in cart");
                }

                // Process removal result (first result set)
                var removalRow = result.Tables[0].Rows[0];
                var removeResponse = new Model.ProductCart.RemoveFromCartResponse
                {
                    CartId = removalRow["CartId"] != DBNull.Value ? Convert.ToInt64(removalRow["CartId"]) : 0,
                    UserId = removalRow["UserId"] != DBNull.Value ? Convert.ToInt64(removalRow["UserId"]) : 0,
                    ProductId = removalRow["ProductId"] != DBNull.Value ? Convert.ToInt64(removalRow["ProductId"]) : 0,
                    ProductName = removalRow["ProductName"]?.ToString() ?? "",
                    RemovedQuantity = removalRow["RemovedQuantity"] != DBNull.Value ? Convert.ToInt32(removalRow["RemovedQuantity"]) : 0,
                    Price = removalRow["Price"] != DBNull.Value ? Convert.ToDecimal(removalRow["Price"]) : 0m,
                    ItemTotal = removalRow["ItemTotal"] != DBNull.Value ? Convert.ToDecimal(removalRow["ItemTotal"]) : 0m,
                    Message = removalRow["Message"]?.ToString() ?? "Product removed from cart successfully",
                    RemovedDate = removalRow["RemovedDate"] != DBNull.Value ? Convert.ToDateTime(removalRow["RemovedDate"]) : DateTime.MinValue
                };

                // Process cart summary (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var summaryRow = result.Tables[1].Rows[0];
                    removeResponse.Summary = new Model.ProductCart.CartSummaryInfo
                    {
                        TotalUniqueItems = summaryRow["TotalUniqueItems"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalUniqueItems"]) : 0,
                        TotalQuantity = summaryRow["TotalQuantity"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalQuantity"]) : 0,
                        TotalAmount = summaryRow["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(summaryRow["TotalAmount"]) : 0m
                    };
                }

                // Process recommended products (third result set)
                if (result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    removeResponse.RecommendedProducts = new List<Model.ProductCart.RecommendedProduct>();
                    foreach (DataRow row in result.Tables[2].Rows)
                    {
                        removeResponse.RecommendedProducts.Add(new Model.ProductCart.RecommendedProduct
                        {
                            ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt64(row["ProductId"]) : 0,
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0m,
                            Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                            BestSeller = row["BestSeller"] != DBNull.Value ? Convert.ToBoolean(row["BestSeller"]) : false,
                            Offer = row["Offer"]?.ToString() ?? "",
                            ImageUrl = row["ImageUrl"]?.ToString() ?? ""
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Remove from cart successful for user {request.UserId}, product {request.ProductId}");

                return removeResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Remove from cart error for user {request.UserId}, product {request.ProductId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                else if (ex.Message.Contains("Product not found in cart"))
                {
                    throw new KeyNotFoundException("Product not found in cart");
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Clear cart for user {request.UserId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_CLEAR_CART,
                    request.UserId,
                    tenantId.ToString() ?? (object)DBNull.Value,
                    request.ClearCompletely,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("User not found or cart is already empty");
                }

                // Process clear cart result (first result set)
                var clearRow = result.Tables[0].Rows[0];
                var clearResponse = new Model.ProductCart.ClearCartResponse
                {
                    UserId = clearRow["UserId"] != DBNull.Value ? Convert.ToInt64(clearRow["UserId"]) : 0,
                    ClearedItemCount = clearRow["ClearedItemCount"] != DBNull.Value ? Convert.ToInt32(clearRow["ClearedItemCount"]) : 0,
                    ClearedQuantity = clearRow["ClearedQuantity"] != DBNull.Value ? Convert.ToInt32(clearRow["ClearedQuantity"]) : 0,
                    ClearedValue = clearRow["ClearedValue"] != DBNull.Value ? Convert.ToDecimal(clearRow["ClearedValue"]) : 0m,
                    Message = clearRow["Message"]?.ToString() ?? "Cart cleared successfully",
                    ClearedDate = clearRow["ClearedDate"] != DBNull.Value ? Convert.ToDateTime(clearRow["ClearedDate"]) : DateTime.MinValue,
                    WasHardDelete = clearRow["WasHardDelete"] != DBNull.Value ? Convert.ToBoolean(clearRow["WasHardDelete"]) : false
                };

                // Process cart summary (second result set - should be empty)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var summaryRow = result.Tables[1].Rows[0];
                    clearResponse.Summary = new Model.ProductCart.CartSummaryInfo
                    {
                        TotalUniqueItems = summaryRow["TotalUniqueItems"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalUniqueItems"]) : 0,
                        TotalQuantity = summaryRow["TotalQuantity"] != DBNull.Value ? Convert.ToInt32(summaryRow["TotalQuantity"]) : 0,
                        TotalAmount = summaryRow["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(summaryRow["TotalAmount"]) : 0m
                    };
                }

                // Process popular products (third result set)
                if (result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    clearResponse.PopularProducts = new List<Model.ProductCart.PopularProduct>();
                    foreach (DataRow row in result.Tables[2].Rows)
                    {
                        clearResponse.PopularProducts.Add(new Model.ProductCart.PopularProduct
                        {
                            ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt64(row["ProductId"]) : 0,
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0m,
                            Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                            BestSeller = row["BestSeller"] != DBNull.Value ? Convert.ToBoolean(row["BestSeller"]) : false,
                            Trending = row["Trending"] != DBNull.Value ? Convert.ToBoolean(row["Trending"]) : false,
                            UserBuyCount = row["UserBuyCount"] != DBNull.Value ? Convert.ToInt32(row["UserBuyCount"]) : 0,
                            Offer = row["Offer"]?.ToString() ?? "",
                            ImageUrl = row["ImageUrl"]?.ToString() ?? "",
                            CategoryName = row["CategoryName"]?.ToString() ?? ""
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Clear cart successful for user {request.UserId} - {clearResponse.ClearedItemCount} items cleared");

                return clearResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Clear cart error for user {request.UserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                else if (ex.Message.Contains("Cart is already empty"))
                {
                    throw new KeyNotFoundException("Cart is already empty");
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Create order for user {request.UserId} with {request.Items.Count} items");

                // Serialize complex objects to JSON for the stored procedure
                var orderItemsJson = System.Text.Json.JsonSerializer.Serialize(request.Items);
                var shippingAddressJson = System.Text.Json.JsonSerializer.Serialize(request.ShippingAddress);
                var paymentMethodJson = System.Text.Json.JsonSerializer.Serialize(request.PaymentMethod);
                var shippingMethodJson = System.Text.Json.JsonSerializer.Serialize(request.ShippingMethod);
                var appliedDiscountJson = request.AppliedDiscount != null 
                    ? System.Text.Json.JsonSerializer.Serialize(request.AppliedDiscount) 
                    : (object)DBNull.Value;

                // The stored procedure expects OUTPUT parameters, but ExecuteDataset doesn't handle them well
                // However, the SP also returns a SELECT statement, so we can get values from the result set
                // We still need to pass the OUTPUT parameters (as DBNull) for the SP to work correctly
                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_CREATE_ORDER,
                    request.UserId,                                    // @UserId
                    request.TenantId,                                  // @TenantId
                    request.SessionId ?? (object)DBNull.Value,         // @SessionId
                    request.OrderType ?? "registered",                // @OrderType
                    request.Status ?? "pending",                       // @OrderStatus
                    request.PaymentStatus ?? "pending",                // @PaymentStatus
                    request.Totals.Subtotal,                          // @Subtotal
                    request.Totals.Shipping,                           // @ShippingAmount
                    request.Totals.Tax,                                // @TaxAmount
                    request.Totals.Discount,                           // @DiscountAmount
                    request.Totals.Total,                              // @TotalAmount
                    request.Totals.Currency ?? "INR",                  // @CurrencyCode
                    request.Notes ?? (object)DBNull.Value,             // @Notes
                    request.SpecialInstructions ?? (object)DBNull.Value, // @SpecialInstructions
                    request.OrderDate ?? (object)DBNull.Value,         // @OrderDate
                    request.Source ?? "web",                            // @Source
                    request.IpAddress ?? (object)DBNull.Value,         // @IpAddress
                    request.UserAgent ?? (object)DBNull.Value,          // @UserAgent
                    request.Referrer ?? (object)DBNull.Value,          // @Referrer
                    shippingAddressJson,                               // @ShippingAddress
                    paymentMethodJson,                                 // @PaymentMethod
                    shippingMethodJson,                               // @ShippingMethod
                    appliedDiscountJson,                               // @AppliedDiscount
                    orderItemsJson,                                     // @OrderItems
                    DBNull.Value,                                       // @OrderNumber OUTPUT
                    DBNull.Value                                        // @OrderId OUTPUT
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Failed to create order - user not found or invalid data");
                }

                // Process order creation result (first result set)
                var orderRow = result.Tables[0].Rows[0];
                var orderResponse = new Model.Order.CreateOrderResponse
                {
                    OrderId = Convert.ToInt64(orderRow["OrderId"]),
                    OrderNumber = orderRow["OrderNumber"]?.ToString() ?? "",
                    UserId = Convert.ToInt64(orderRow["UserId"]),
                    ItemCount = Convert.ToInt32(orderRow["ItemCount"]),
                    TotalAmount = Convert.ToDecimal(orderRow["TotalAmount"]),
                    OrderStatus = orderRow["OrderStatus"]?.ToString() ?? "",
                    PaymentStatus = orderRow["PaymentStatus"]?.ToString() ?? "",
                    Message = orderRow["Message"]?.ToString() ?? "Order created successfully",
                    CreatedDate = Convert.ToDateTime(orderRow["CreatedDate"])
                };

                // Process order summary (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var summaryRow = result.Tables[1].Rows[0];
                    orderResponse.Summary = new Model.Order.OrderSummaryInfo
                    {
                        OrderId = Convert.ToInt64(summaryRow["OrderId"]),
                        OrderNumber = summaryRow["OrderNumber"]?.ToString() ?? "",
                        TotalAmount = Convert.ToDecimal(summaryRow["TotalAmount"]),
                        OrderStatus = summaryRow["OrderStatus"]?.ToString() ?? "",
                        PaymentStatus = summaryRow["PaymentStatus"]?.ToString() ?? "",
                        CreatedAt = Convert.ToDateTime(summaryRow["CreatedAt"]),
                        TotalItems = Convert.ToInt32(summaryRow["TotalItems"]),
                        TotalQuantity = Convert.ToInt32(summaryRow["TotalQuantity"])
                    };
                }

                // Process order items (third result set)
                if (result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    orderResponse.Items = new List<Model.Order.OrderItemInfo>();
                    foreach (DataRow row in result.Tables[2].Rows)
                    {
                        orderResponse.Items.Add(new Model.Order.OrderItemInfo
                        {
                            OrderItemId = Convert.ToInt64(row["OrderItemId"]),
                            ProductId = Convert.ToInt64(row["ProductId"]),
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            ProductImage = row["ProductImage"]?.ToString() ?? "",
                            Price = Convert.ToDecimal(row["Price"]),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            Total = Convert.ToDecimal(row["Total"]),
                            ProductCode = row["ProductCode"]?.ToString() ?? "",
                            Category = row["Category"]?.ToString() ?? ""
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Create order successful for user {request.UserId}, Order Number: {orderResponse.OrderNumber}");

                return orderResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Create order error for user {request.UserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                else if (ex.Message.Contains("Order must contain at least one item"))
                {
                    throw new ArgumentException("Order must contain at least one item");
                }
                else if (ex.Message.Contains("Insufficient stock"))
                {
                    throw new InvalidOperationException(ex.Message);
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Get orders for user {request.UserId}, page {request.Page}, limit {request.Limit}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_GET_ORDERS,
                    request.UserId,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Page,
                    request.Limit,
                    request.Status ?? (object)DBNull.Value,
                    request.Search ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    throw new KeyNotFoundException("User not found or no orders available");
                }

                var ordersResponse = new Model.Order.GetOrdersResponse();

                // Process orders (first result set)
                if (result.Tables[0].Rows.Count > 0)
                {
                    var ordersList = new List<Model.Order.OrderListItem>();
                    var paginationInfo = new Model.Order.PaginationInfo();

                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = new Model.Order.OrderListItem
                        {
                            Id = Convert.ToInt64(row["OrderId"]),
                            OrderNumber = row["OrderNumber"]?.ToString() ?? "",
                            Status = row["OrderStatus"]?.ToString() ?? "",
                            PaymentStatus = row["PaymentStatus"]?.ToString() ?? "",
                            Total = Convert.ToDecimal(row["TotalAmount"]),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),
                            ShippingAmount = Convert.ToDecimal(row["ShippingAmount"]),
                            TaxAmount = Convert.ToDecimal(row["TaxAmount"]),
                            DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                            Notes = row["Notes"]?.ToString() ?? "",
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                            TotalItems = Convert.ToInt32(row["TotalItems"]),
                            TotalQuantity = Convert.ToInt32(row["TotalQuantity"])
                        };

                        ordersList.Add(order);

                        // Set pagination info (same for all rows)
                        if (paginationInfo.Total == 0)
                        {
                            paginationInfo.Page = Convert.ToInt32(row["CurrentPage"]);
                            paginationInfo.Limit = Convert.ToInt32(row["PageSize"]);
                            paginationInfo.Total = Convert.ToInt32(row["TotalCount"]);
                            paginationInfo.TotalPages = Convert.ToInt32(row["TotalPages"]);
                            paginationInfo.HasNext = Convert.ToBoolean(row["HasNext"]);
                            paginationInfo.HasPrevious = Convert.ToBoolean(row["HasPrevious"]);
                        }
                    }

                    ordersResponse.Orders = ordersList;
                    ordersResponse.Pagination = paginationInfo;
                }

                // Process order items (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var orderItemsDict = new Dictionary<long, List<Model.Order.OrderListItemInfo>>();

                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        var orderId = Convert.ToInt64(row["OrderId"]);
                        var orderItem = new Model.Order.OrderListItemInfo
                        {
                            OrderItemId = Convert.ToInt64(row["OrderItemId"]),
                            ProductId = Convert.ToInt64(row["ProductId"]),
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            ProductImage = row["ProductImage"]?.ToString() ?? "",
                            Price = Convert.ToDecimal(row["Price"]),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            Total = Convert.ToDecimal(row["Total"]),
                            ProductCode = row["ProductCode"]?.ToString() ?? "",
                            Category = row["Category"]?.ToString() ?? "",
                            Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                            Offer = row["Offer"]?.ToString() ?? ""
                        };

                        if (!orderItemsDict.ContainsKey(orderId))
                        {
                            orderItemsDict[orderId] = new List<Model.Order.OrderListItemInfo>();
                        }
                        orderItemsDict[orderId].Add(orderItem);
                    }

                    // Assign items to their respective orders
                    foreach (var order in ordersResponse.Orders)
                    {
                        if (orderItemsDict.ContainsKey(order.Id))
                        {
                            order.Items = orderItemsDict[order.Id];
                        }
                    }
                }

                this.Logger.LogInformation($"Repository: Get orders successful for user {request.UserId}, found {ordersResponse.Orders.Count} orders");

                return ordersResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Get orders error for user {request.UserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Get order by ID for user {request.UserId}, order {request.OrderId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_GET_ORDER_BY_ID,
                    request.OrderId,
                    request.UserId,
                    request.TenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Order not found or does not belong to user");
                }

                // Process order details (first result set)
                var orderRow = result.Tables[0].Rows[0];
                var orderResponse = new Model.Order.GetOrderByIdResponse
                {
                    OrderId = Convert.ToInt64(orderRow["OrderId"]),
                    OrderNumber = orderRow["OrderNumber"]?.ToString() ?? "",
                    OrderStatus = orderRow["OrderStatus"]?.ToString() ?? "",
                    PaymentStatus = orderRow["PaymentStatus"]?.ToString() ?? "",
                    TotalAmount = Convert.ToDecimal(orderRow["TotalAmount"]),
                    Subtotal = Convert.ToDecimal(orderRow["Subtotal"]),
                    ShippingAmount = Convert.ToDecimal(orderRow["ShippingAmount"]),
                    TaxAmount = Convert.ToDecimal(orderRow["TaxAmount"]),
                    DiscountAmount = Convert.ToDecimal(orderRow["DiscountAmount"]),
                    Notes = orderRow["Notes"]?.ToString() ?? "",
                    ShippingAddress = orderRow["ShippingAddress"]?.ToString() ?? "",
                    BillingAddress = orderRow["BillingAddress"]?.ToString() ?? "",
                    PaymentMethod = orderRow["PaymentMethod"]?.ToString() ?? "",
                    ShippingMethod = orderRow["ShippingMethod"]?.ToString() ?? "",
                    CreatedAt = Convert.ToDateTime(orderRow["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(orderRow["UpdatedAt"]),
                    CustomerName = orderRow["CustomerName"]?.ToString() ?? "",
                    CustomerEmail = orderRow["CustomerEmail"]?.ToString() ?? "",
                    CustomerPhone = orderRow["CustomerPhone"]?.ToString() ?? "",
                    TotalItems = Convert.ToInt32(orderRow["TotalItems"]),
                    TotalQuantity = Convert.ToInt32(orderRow["TotalQuantity"])
                };

                // Process order items (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    orderResponse.Items = new List<Model.Order.OrderDetailItemInfo>();
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        orderResponse.Items.Add(new Model.Order.OrderDetailItemInfo
                        {
                            OrderItemId = Convert.ToInt64(row["OrderItemId"]),
                            ProductId = Convert.ToInt64(row["ProductId"]),
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            ProductImage = row["ProductImage"]?.ToString() ?? "",
                            Price = Convert.ToDecimal(row["Price"]),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            Total = Convert.ToDecimal(row["Total"]),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            ProductCode = row["ProductCode"]?.ToString() ?? "",
                            ProductDescription = row["ProductDescription"]?.ToString() ?? "",
                            Category = row["Category"]?.ToString() ?? "",
                            Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                            Offer = row["Offer"]?.ToString() ?? "",
                            InStock = row["InStock"] != DBNull.Value ? Convert.ToBoolean(row["InStock"]) : false,
                            BestSeller = row["BestSeller"] != DBNull.Value ? Convert.ToBoolean(row["BestSeller"]) : false
                        });
                    }
                }

                // Process order status history (third result set)
                if (result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    orderResponse.StatusHistory = new List<Model.Order.OrderStatusHistoryInfo>();
                    foreach (DataRow row in result.Tables[2].Rows)
                    {
                        orderResponse.StatusHistory.Add(new Model.Order.OrderStatusHistoryInfo
                        {
                            StatusHistoryId = Convert.ToInt64(row["StatusHistoryId"]),
                            PreviousStatus = row["PreviousStatus"]?.ToString() ?? "",
                            NewStatus = row["NewStatus"]?.ToString() ?? "",
                            StatusNote = row["StatusNote"]?.ToString() ?? "",
                            ChangedBy = Convert.ToInt64(row["ChangedBy"]),
                            ChangedByName = row["ChangedByName"]?.ToString() ?? "",
                            ChangedAt = Convert.ToDateTime(row["ChangedAt"])
                        });
                    }
                }

                // Process order tracking information (fourth result set)
                if (result.Tables.Count > 3 && result.Tables[3].Rows.Count > 0)
                {
                    orderResponse.TrackingInfo = new List<Model.Order.OrderTrackingInfo>();
                    foreach (DataRow row in result.Tables[3].Rows)
                    {
                        orderResponse.TrackingInfo.Add(new Model.Order.OrderTrackingInfo
                        {
                            TrackingId = Convert.ToInt64(row["TrackingId"]),
                            TrackingNumber = row["TrackingNumber"]?.ToString() ?? "",
                            Carrier = row["Carrier"]?.ToString() ?? "",
                            TrackingStatus = row["TrackingStatus"]?.ToString() ?? "",
                            EstimatedDelivery = row["EstimatedDelivery"] != DBNull.Value ? Convert.ToDateTime(row["EstimatedDelivery"]) : null,
                            ActualDelivery = row["ActualDelivery"] != DBNull.Value ? Convert.ToDateTime(row["ActualDelivery"]) : null,
                            TrackingUrl = row["TrackingUrl"]?.ToString() ?? "",
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Get order by ID successful for user {request.UserId}, order {request.OrderId}");

                return orderResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Get order by ID error for user {request.UserId}, order {request.OrderId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                else if (ex.Message.Contains("Order not found or does not belong to user"))
                {
                    throw new KeyNotFoundException("Order not found or does not belong to user");
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Cancel order for user {request.UserId}, order {request.OrderId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_CANCEL_ORDER,
                    request.OrderId,
                    request.UserId,
                    request.TenantId ?? (object)DBNull.Value,
                    request.CancelReason ?? (object)DBNull.Value,
                    request.CancelledBy ?? (object)DBNull.Value,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Order not found or does not belong to user");
                }

                // Process cancellation result (first result set)
                var cancelRow = result.Tables[0].Rows[0];
                var cancelResponse = new Model.Order.CancelOrderResponse
                {
                    OrderId = Convert.ToInt64(cancelRow["OrderId"]),
                    OrderNumber = cancelRow["OrderNumber"]?.ToString() ?? "",
                    UserId = Convert.ToInt64(cancelRow["UserId"]),
                    PreviousStatus = cancelRow["PreviousStatus"]?.ToString() ?? "",
                    NewStatus = cancelRow["NewStatus"]?.ToString() ?? "",
                    RefundAmount = Convert.ToDecimal(cancelRow["RefundAmount"]),
                    CancelReason = cancelRow["CancelReason"]?.ToString() ?? "",
                    Message = cancelRow["Message"]?.ToString() ?? "Order cancelled successfully",
                    CancelledDate = Convert.ToDateTime(cancelRow["CancelledDate"]),
                    RefundInitiated = Convert.ToBoolean(cancelRow["RefundInitiated"]),
                    Success = true
                };

                this.Logger.LogInformation($"Repository: Cancel order successful for user {request.UserId}, order {request.OrderId}");

                return cancelResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Cancel order error for user {request.UserId}, order {request.OrderId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                else if (ex.Message.Contains("Order not found or does not belong to user"))
                {
                    throw new KeyNotFoundException("Order not found or does not belong to user");
                }
                else if (ex.Message.Contains("Order cannot be cancelled in current status"))
                {
                    throw new InvalidOperationException(ex.Message);
                }
                
                throw;
            }
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
                this.Logger.LogInformation($"Repository: Update order status for order {request.OrderId} to {request.Status}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_UPDATE_ORDER_STATUS,
                    request.OrderId,
                    request.UserId ?? (object)DBNull.Value,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Status,
                    request.Note ?? (object)DBNull.Value,
                    request.TrackingNumber ?? (object)DBNull.Value,
                    request.Carrier ?? (object)DBNull.Value,
                    request.EstimatedDelivery ?? (object)DBNull.Value,
                    request.UpdatedBy ?? (object)DBNull.Value,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Order not found or access denied");
                }

                // Process status update result (first result set)
                var statusRow = result.Tables[0].Rows[0];
                var statusResponse = new Model.Order.UpdateOrderStatusResponse
                {
                    OrderId = Convert.ToInt64(statusRow["OrderId"]),
                    OrderNumber = statusRow["OrderNumber"]?.ToString() ?? "",
                    UserId = Convert.ToInt64(statusRow["UserId"]),
                    PreviousStatus = statusRow["PreviousStatus"]?.ToString() ?? "",
                    NewStatus = statusRow["NewStatus"]?.ToString() ?? "",
                    TrackingNumber = statusRow["TrackingNumber"]?.ToString() ?? "",
                    Carrier = statusRow["Carrier"]?.ToString() ?? "",
                    EstimatedDelivery = statusRow["EstimatedDelivery"] != DBNull.Value ? Convert.ToDateTime(statusRow["EstimatedDelivery"]) : null,
                    StatusNote = statusRow["StatusNote"]?.ToString() ?? "",
                    Message = statusRow["Message"]?.ToString() ?? "Order status updated successfully",
                    UpdatedDate = Convert.ToDateTime(statusRow["UpdatedDate"]),
                    Success = true
                };

                // Process tracking information (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var trackingRow = result.Tables[1].Rows[0];
                    statusResponse.TrackingInfo = new Model.Order.OrderTrackingInfo
                    {
                        TrackingId = Convert.ToInt64(trackingRow["TrackingId"]),
                        TrackingNumber = trackingRow["TrackingNumber"]?.ToString() ?? "",
                        Carrier = trackingRow["Carrier"]?.ToString() ?? "",
                        TrackingStatus = trackingRow["TrackingStatus"]?.ToString() ?? "",
                        EstimatedDelivery = trackingRow["EstimatedDelivery"] != DBNull.Value ? Convert.ToDateTime(trackingRow["EstimatedDelivery"]) : null,
                        ActualDelivery = trackingRow["ActualDelivery"] != DBNull.Value ? Convert.ToDateTime(trackingRow["ActualDelivery"]) : null,
                        TrackingUrl = trackingRow["TrackingUrl"]?.ToString() ?? "",
                        CreatedAt = Convert.ToDateTime(trackingRow["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(trackingRow["UpdatedAt"])
                    };
                }

                this.Logger.LogInformation($"Repository: Update order status successful for order {request.OrderId} to {request.Status}");

                return statusResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Update order status error for order {request.OrderId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }
                else if (ex.Message.Contains("Order not found or access denied"))
                {
                    throw new KeyNotFoundException("Order not found or access denied");
                }
                else if (ex.Message.Contains("Invalid status transition"))
                {
                    throw new InvalidOperationException(ex.Message);
                }
                
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
                this.Logger.LogInformation($"Repository: Admin get all users by admin {request.AdminUserId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_ADMIN_GET_ALL_USERS,
                    request.AdminUserId,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Page,
                    request.Limit,
                    request.Search ?? (object)DBNull.Value,
                    request.Role ?? (object)DBNull.Value,
                    request.Status ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    throw new UnauthorizedAccessException("Admin user not found or insufficient privileges");
                }

                var usersResponse = new Model.Admin.GetAllUsersResponse();

                // Process users (first result set)
                if (result.Tables[0].Rows.Count > 0)
                {
                    var usersList = new List<Model.Admin.AdminUserInfo>();
                    var paginationInfo = new Model.Order.PaginationInfo();

                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var user = new Model.Admin.AdminUserInfo
                        {
                            UserId = row["Id"] != DBNull.Value ? Convert.ToInt64(row["Id"]) : 0,
                            FullName = row["Name"]?.ToString() ?? "",
                            Email = row["Email"]?.ToString() ?? "",
                            Phone = row["Phone"]?.ToString() ?? "",
                            Role = row["Role"]?.ToString() ?? "",
                            EmailVerified = Convert.ToBoolean(row["EmailVerified"]),
                            PhoneVerified = Convert.ToBoolean(row["PhoneVerified"]),
                            Status = row["Status"]?.ToString() ?? "",
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : null,
                            OrderCount = Convert.ToInt32(row["OrderCount"]),
                            TotalSpent = Convert.ToDecimal(row["TotalSpent"]),
                            LastOrderDate = row["LastOrderDate"] != DBNull.Value ? Convert.ToDateTime(row["LastOrderDate"]) : null,
                            TenantId = row["TenantId"] != DBNull.Value ? Convert.ToInt64(row["TenantId"]) : null,
                            DateOfBirth = row["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(row["DateOfBirth"]) : null,
                            Gender = row["Gender"]?.ToString() ?? "",
                            Country = row["Country"]?.ToString() ?? "",
                            City = row["City"]?.ToString() ?? "",
                            State = row["State"]?.ToString() ?? "",
                            PostalCode = row["PostalCode"]?.ToString() ?? "",
                            CompanyName = row["CompanyName"]?.ToString() ?? "",
                            JobTitle = row["JobTitle"]?.ToString() ?? ""
                        };

                        usersList.Add(user);

                        // Set pagination info (same for all rows)
                        if (paginationInfo.Total == 0)
                        {
                            paginationInfo.Page = Convert.ToInt32(row["CurrentPage"]);
                            paginationInfo.Limit = Convert.ToInt32(row["PageSize"]);
                            paginationInfo.Total = Convert.ToInt32(row["TotalCount"]);
                            paginationInfo.TotalPages = Convert.ToInt32(row["TotalPages"]);
                            paginationInfo.HasNext = Convert.ToBoolean(row["HasNext"]);
                            paginationInfo.HasPrevious = Convert.ToBoolean(row["HasPrevious"]);
                        }
                    }

                    usersResponse.Users = usersList;
                    usersResponse.Pagination = paginationInfo;
                }

                // Process user permissions (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var userPermissionsDict = new Dictionary<string, List<Model.Admin.UserPermissionInfo>>();

                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        var userId = row["UserId"]?.ToString() ?? "";
                        var permission = new Model.Admin.UserPermissionInfo
                        {
                            PermissionName = row["PermissionName"]?.ToString() ?? "",
                            PermissionDescription = row["PermissionDescription"]?.ToString() ?? "",
                            PermissionSource = "Role-based" // Default for this result set
                        };

                        if (!userPermissionsDict.ContainsKey(userId))
                        {
                            userPermissionsDict[userId] = new List<Model.Admin.UserPermissionInfo>();
                        }
                        userPermissionsDict[userId].Add(permission);
                    }

                    // Assign permissions to their respective users
                    foreach (var user in usersResponse.Users)
                    {
                        var userIdStr = user.UserId.ToString();
                        if (userPermissionsDict.ContainsKey(userIdStr))
                        {
                            user.Permissions = userPermissionsDict[userIdStr];
                        }
                    }
                }

                this.Logger.LogInformation($"Repository: Admin get all users successful by admin {request.AdminUserId}, found {usersResponse.Users.Count} users");

                return usersResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Admin get all users error by admin {request.AdminUserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or insufficient privileges"))
                {
                    throw new UnauthorizedAccessException("User not found or insufficient privileges");
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Admin update user role by admin {request.AdminUserId} for user {request.UserId} to {request.Role}");

                // Convert permissions list to JSON if provided
                string permissionsJson = null;
                if (request.Permissions != null && request.Permissions.Any())
                {
                    permissionsJson = System.Text.Json.JsonSerializer.Serialize(request.Permissions);
                }

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_ADMIN_UPDATE_USER_ROLE,
                    request.AdminUserId,
                    request.UserId,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Role,
                    permissionsJson ?? (object)DBNull.Value,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Admin user not found or insufficient privileges");
                }

                // Process role update result (first result set)
                var roleRow = result.Tables[0].Rows[0];
                var roleResponse = new Model.Admin.UpdateUserRoleResponse
                {
                    UserId = Convert.ToInt64(roleRow["UserId"]),
                    UserName = roleRow["UserName"]?.ToString() ?? "",
                    PreviousRole = roleRow["PreviousRole"]?.ToString() ?? "",
                    NewRole = roleRow["NewRole"]?.ToString() ?? "",
                    UpdatedBy = Convert.ToInt64(roleRow["UpdatedBy"]),
                    UpdatedAt = Convert.ToDateTime(roleRow["UpdatedAt"]),
                    Message = roleRow["Message"]?.ToString() ?? "User role updated successfully",
                    Success = true
                };

                // Process assigned permissions (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    roleResponse.AssignedPermissions = new List<Model.Admin.UserPermissionInfo>();
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        roleResponse.AssignedPermissions.Add(new Model.Admin.UserPermissionInfo
                        {
                            PermissionName = row["PermissionName"]?.ToString() ?? "",
                            PermissionDescription = row["PermissionDescription"]?.ToString() ?? "",
                            PermissionSource = row["PermissionSource"]?.ToString() ?? "Role-based"
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Admin update user role successful by admin {request.AdminUserId} for user {request.UserId} to {request.Role}");

                return roleResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Admin update user role error by admin {request.AdminUserId} for user {request.UserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Admin user not found or insufficient privileges"))
                {
                    throw new UnauthorizedAccessException("Admin user not found or insufficient privileges");
                }
                else if (ex.Message.Contains("Target user not found or inactive"))
                {
                    throw new KeyNotFoundException("Target user not found or inactive");
                }
                else if (ex.Message.Contains("Role not found"))
                {
                    throw new ArgumentException(ex.Message);
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Admin get all orders by admin {request.AdminUserId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_ADMIN_GET_ALL_ORDERS,
                    request.AdminUserId,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Page,
                    request.Limit,
                    request.Status ?? (object)DBNull.Value,
                    request.Search ?? (object)DBNull.Value,
                    request.StartDate ?? (object)DBNull.Value,
                    request.EndDate ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    throw new UnauthorizedAccessException("Admin user not found or insufficient privileges");
                }

                var ordersResponse = new Model.Admin.GetAllOrdersResponse();

                // Process orders (first result set)
                if (result.Tables[0].Rows.Count > 0)
                {
                    var ordersList = new List<Model.Admin.AdminOrderInfo>();
                    var paginationInfo = new Model.Order.PaginationInfo();

                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = new Model.Admin.AdminOrderInfo
                        {
                            OrderId = row["OrderId"]?.ToString() ?? "",
                            OrderNumber = row["OrderNumber"]?.ToString() ?? "",
                            CustomerName = row["CustomerName"]?.ToString() ?? "",
                            CustomerEmail = row["CustomerEmail"]?.ToString() ?? "",
                            CustomerPhone = row["CustomerPhone"]?.ToString() ?? "",
                            Status = row["Status"]?.ToString() ?? "",
                            PaymentStatus = row["PaymentStatus"]?.ToString() ?? "",
                            Total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0m,
                            Subtotal = row["Subtotal"] != DBNull.Value ? Convert.ToDecimal(row["Subtotal"]) : 0m,
                            ShippingAmount = row["ShippingAmount"] != DBNull.Value ? Convert.ToDecimal(row["ShippingAmount"]) : 0m,
                            TaxAmount = row["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(row["TaxAmount"]) : 0m,
                            DiscountAmount = row["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["DiscountAmount"]) : 0m,
                            ItemCount = row["ItemCount"] != DBNull.Value ? Convert.ToInt32(row["ItemCount"]) : 0,
                            TotalQuantity = row["TotalQuantity"] != DBNull.Value ? Convert.ToInt32(row["TotalQuantity"]) : 0,
                            CreatedAt = row["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(row["CreatedAt"]) : DateTime.MinValue,
                            UpdatedAt = row["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedAt"]) : DateTime.MinValue,
                            ShippedAt = row["ShippedAt"] != DBNull.Value ? Convert.ToDateTime(row["ShippedAt"]) : null,
                            DeliveredAt = row["DeliveredAt"] != DBNull.Value ? Convert.ToDateTime(row["DeliveredAt"]) : null,
                            EstimatedDelivery = row["EstimatedDelivery"] != DBNull.Value ? Convert.ToDateTime(row["EstimatedDelivery"]) : null,
                            ShippingAddress = DeserializeJsonOrDefault<Model.Order.AddressRequest>(row["ShippingAddress"]?.ToString()),
                            PaymentMethod = DeserializeJsonOrDefault<Model.Order.PaymentMethodRequest>(row["PaymentMethod"]?.ToString()),
                            ShippingMethod = DeserializeJsonOrDefault<Model.Order.ShippingMethodRequest>(row["ShippingMethod"]?.ToString()),
                            Notes = row["Notes"]?.ToString() ?? "",
                            CustomerId = row["CustomerId"] != DBNull.Value ? Convert.ToInt64(row["CustomerId"]) : 0,
                            CustomerTenantId = row["CustomerTenantId"] != DBNull.Value ? Convert.ToInt64(row["CustomerTenantId"]) : null
                        };

                        ordersList.Add(order);

                        // Set pagination info (same for all rows)
                        if (paginationInfo.Total == 0)
                        {
                            paginationInfo.Page = Convert.ToInt32(row["CurrentPage"]);
                            paginationInfo.Limit = Convert.ToInt32(row["PageSize"]);
                            paginationInfo.Total = Convert.ToInt32(row["TotalCount"]);
                            paginationInfo.TotalPages = Convert.ToInt32(row["TotalPages"]);
                            paginationInfo.HasNext = Convert.ToBoolean(row["HasNext"]);
                            paginationInfo.HasPrevious = Convert.ToBoolean(row["HasPrevious"]);
                        }
                    }

                    ordersResponse.Orders = ordersList;
                    ordersResponse.Pagination = paginationInfo;
                }

                // Process order items (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    var orderItemsDict = new Dictionary<string, List<Model.Admin.AdminOrderItemInfo>>();

                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        var orderId = row["OrderId"]?.ToString() ?? "";
                        var orderItem = new Model.Admin.AdminOrderItemInfo
                        {
                            OrderItemId = row["OrderItemId"] != DBNull.Value ? Convert.ToInt64(row["OrderItemId"]) : 0,
                            ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt64(row["ProductId"]) : 0,
                            ProductName = row["ProductName"]?.ToString() ?? "",
                            ProductImage = row["ProductImage"]?.ToString() ?? "",
                            Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0m,
                            Quantity = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0,
                            Total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0m,
                            ProductCode = row["ProductCode"]?.ToString() ?? "",
                            Category = row["Category"]?.ToString() ?? "",
                            Rating = row["Rating"] != DBNull.Value ? Convert.ToDecimal(row["Rating"]) : 0,
                            Offer = row["Offer"]?.ToString() ?? ""
                        };

                        if (!orderItemsDict.ContainsKey(orderId))
                        {
                            orderItemsDict[orderId] = new List<Model.Admin.AdminOrderItemInfo>();
                        }
                        orderItemsDict[orderId].Add(orderItem);
                    }

                    // Assign items to their respective orders
                    foreach (var order in ordersResponse.Orders)
                    {
                        if (orderItemsDict.ContainsKey(order.OrderId))
                        {
                            order.Items = orderItemsDict[order.OrderId];
                        }
                    }
                }

                // Process order statistics (third result set)
                if (result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    var statsRow = result.Tables[2].Rows[0];
                    ordersResponse.Statistics = new Model.Admin.OrderStatsSummary
                    {
                        TotalOrders = statsRow["TotalOrders"] != DBNull.Value ? Convert.ToInt32(statsRow["TotalOrders"]) : 0,
                        TotalRevenue = statsRow["TotalRevenue"] != DBNull.Value ? Convert.ToDecimal(statsRow["TotalRevenue"]) : 0m,
                        AverageOrderValue = statsRow["AverageOrderValue"] != DBNull.Value ? Convert.ToDecimal(statsRow["AverageOrderValue"]) : 0m,
                        UniqueCustomers = statsRow["UniqueCustomers"] != DBNull.Value ? Convert.ToInt32(statsRow["UniqueCustomers"]) : 0,
                        PendingOrders = statsRow["PendingOrders"] != DBNull.Value ? Convert.ToInt32(statsRow["PendingOrders"]) : 0,
                        ProcessingOrders = statsRow["ProcessingOrders"] != DBNull.Value ? Convert.ToInt32(statsRow["ProcessingOrders"]) : 0,
                        ShippedOrders = statsRow["ShippedOrders"] != DBNull.Value ? Convert.ToInt32(statsRow["ShippedOrders"]) : 0,
                        DeliveredOrders = statsRow["DeliveredOrders"] != DBNull.Value ? Convert.ToInt32(statsRow["DeliveredOrders"]) : 0,
                        CancelledOrders = statsRow["CancelledOrders"] != DBNull.Value ? Convert.ToInt32(statsRow["CancelledOrders"]) : 0
                    };
                }

                this.Logger.LogInformation($"Repository: Admin get all orders successful by admin {request.AdminUserId}, found {ordersResponse.Orders.Count} orders");

                return ordersResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Admin get all orders error by admin {request.AdminUserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or insufficient privileges"))
                {
                    throw new UnauthorizedAccessException("User not found or insufficient privileges");
                }
                
                throw;
            }
        }

        /// <summary>
        /// Add multiple images to a product
        /// </summary>
        /// <param name="request">Add product images request</param>
        /// <param name="imageDataList">List of processed image data</param>
        /// <returns>List of added images</returns>
        public async Task<Model.Product.AddProductImagesResponse> AddProductImages(Model.Product.AddProductImagesRequest request, List<Tenant.Query.Model.Product.ImageUploadData> imageDataList)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Add product images for product {request.ProductId}, count: {imageDataList.Count}");

                // Convert image data list to XML for stored procedure (SQL Server 2014 compatible)
                string imagesXml = ConvertImageDataToXml(imageDataList);
                
                // Log the XML being sent to stored procedure for debugging
                this.Logger.LogInformation($"Repository: Sending XML to stored procedure: {imagesXml}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_ADD_PRODUCT_IMAGES,
                    request.ProductId,
                    request.UserId ?? (object)DBNull.Value,
                    request.TenantId ?? (object)DBNull.Value,
                    imagesXml,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Product not found or no images were added");
                }

                // Process added images (first result set)
                var addedImages = new List<Model.Product.ProductImageInfo>();
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    var imageId = Convert.ToInt64(row["ImageId"]);
                    var imageUrl = GenerateImageUrl(imageId);
                    var thumbnailUrl = GenerateThumbnailUrl(imageId);
                    
                    // Log the generated URLs for debugging
                    this.Logger.LogInformation($"Repository: Generated URLs for image {imageId} - Image: {imageUrl}, Thumbnail: {thumbnailUrl}");
                    
                    var imageInfo = new Model.Product.ProductImageInfo
                    {
                        ImageId = imageId,
                        ProductId = Convert.ToInt64(row["ProductId"]),
                        Poster = imageUrl, // Generate proper API URL
                        ThumbnailUrl = thumbnailUrl, // Generate proper thumbnail API URL
                        Main = Convert.ToBoolean(row["Main"]),
                        Active = Convert.ToBoolean(row["Active"]),
                        OrderBy = Convert.ToInt32(row["OrderBy"]),
                        Created = Convert.ToDateTime(row["Created"]),
                        Modified = Convert.ToDateTime(row["Modified"]),
                        ImageName = row["ImageName"]?.ToString() ?? "",
                        ContentType = row["ContentType"]?.ToString() ?? "",
                        FileSize = Convert.ToInt64(row["FileSize"])
                    };
                    
                    // Log the created object for debugging
                    this.Logger.LogInformation($"Repository: Created ImageInfo - ImageId: {imageInfo.ImageId}, Poster: '{imageInfo.Poster}', ThumbnailUrl: '{imageInfo.ThumbnailUrl}'");
                    
                    addedImages.Add(imageInfo);
                }

                var response = new Model.Product.AddProductImagesResponse
                {
                    Images = addedImages,
                    TotalAdded = addedImages.Count,
                    Message = $"Successfully added {addedImages.Count} images to product",
                    Success = true
                };

                this.Logger.LogInformation($"Repository: Add product images successful for product {request.ProductId}, added: {addedImages.Count}");

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Add product images error for product {request.ProductId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Product not found or inactive"))
                {
                    throw new KeyNotFoundException("Product not found or inactive");
                }
                else if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new ArgumentException("User not found or inactive");
                }
                else if (ex.Message.Contains("At least one image is required"))
                {
                    throw new ArgumentException("At least one image is required");
                }
                
                throw;
            }
        }

        /// <summary>
        /// Generate proper API URL for image retrieval
        /// </summary>
        /// <param name="imageId">Image ID</param>
        /// <returns>Full API URL for image</returns>
        private string GenerateImageUrl(long imageId)
        {
            // This will generate a URL like: /api/1.0/products/{imageId}
            // The base URL will be added by the client or reverse proxy
            return $"/api/1.0/products/{imageId}";
        }

        /// <summary>
        /// Generate proper API URL for thumbnail image retrieval
        /// </summary>
        /// <param name="imageId">Image ID</param>
        /// <returns>Full API URL for thumbnail image</returns>
        private string GenerateThumbnailUrl(long imageId)
        {
            // This will generate a URL like: /api/1.0/products/{imageId}/thumbnail
            return $"/api/1.0/products/{imageId}/thumbnail";
        }

        /// <summary>
        /// Convert image data list to XML format for SQL Server 2014 compatibility
        /// </summary>
        /// <param name="imageDataList">List of image data</param>
        /// <returns>XML string representation</returns>
        private string ConvertImageDataToXml(List<Tenant.Query.Model.Product.ImageUploadData> imageDataList)
        {
            var xmlBuilder = new System.Text.StringBuilder();
            xmlBuilder.Append("<Images>");
            
            foreach (var imageData in imageDataList)
            {
                xmlBuilder.Append("<Image>");
                xmlBuilder.AppendFormat("<ImageName>{0}</ImageName>", System.Security.SecurityElement.Escape(imageData.ImageName ?? ""));
                xmlBuilder.AppendFormat("<ContentType>{0}</ContentType>", System.Security.SecurityElement.Escape(imageData.ContentType ?? ""));
                xmlBuilder.AppendFormat("<FileSize>{0}</FileSize>", imageData.FileSize);
                
                // ImageData and ThumbnailData are already base64 strings in the ImageUploadData model
                string imageDataBase64 = imageData.ImageData ?? "";
                string thumbnailDataBase64 = imageData.ThumbnailData ?? "";
                
                xmlBuilder.AppendFormat("<ImageData>{0}</ImageData>", imageDataBase64);
                xmlBuilder.AppendFormat("<ThumbnailData>{0}</ThumbnailData>", thumbnailDataBase64);
                xmlBuilder.AppendFormat("<IsMain>{0}</IsMain>", imageData.IsMain ? "1" : "0");
                xmlBuilder.AppendFormat("<OrderBy>{0}</OrderBy>", imageData.OrderBy);
                xmlBuilder.Append("</Image>");
            }
            
            xmlBuilder.Append("</Images>");
            return xmlBuilder.ToString();
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
                this.Logger.LogInformation($"Repository: Update product image for product {request.ProductId}, image {request.ImageId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_UPDATE_PRODUCT_IMAGE,
                    request.ProductId,
                    request.ImageId,
                    request.UserId ?? (object)DBNull.Value,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Main ?? (object)DBNull.Value,
                    request.Active ?? (object)DBNull.Value,
                    request.OrderBy ?? (object)DBNull.Value,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Product or image not found");
                }

                // Process updated image (first result set)
                var imageRow = result.Tables[0].Rows[0];
                var updatedImage = new Model.Product.ProductImageInfo
                {
                    ImageId = Convert.ToInt64(imageRow["ImageId"]),
                    ProductId = Convert.ToInt64(imageRow["ProductId"]),
                    Poster = imageRow["Poster"]?.ToString() ?? "",
                    Main = Convert.ToBoolean(imageRow["Main"]),
                    Active = Convert.ToBoolean(imageRow["Active"]),
                    OrderBy = Convert.ToInt32(imageRow["OrderBy"]),
                    Created = Convert.ToDateTime(imageRow["Created"]),
                    Modified = Convert.ToDateTime(imageRow["Modified"]),
                    ImageName = imageRow["ImageName"]?.ToString() ?? "",
                    ContentType = imageRow["ContentType"]?.ToString() ?? "",
                    FileSize = Convert.ToInt64(imageRow["FileSize"])
                };

                var response = new Model.Product.UpdateProductImageResponse
                {
                    Image = updatedImage,
                    Message = imageRow["Message"]?.ToString() ?? "Image updated successfully",
                    Success = true
                };

                this.Logger.LogInformation($"Repository: Update product image successful for product {request.ProductId}, image {request.ImageId}");

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Update product image error for product {request.ProductId}, image {request.ImageId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Product not found or inactive"))
                {
                    throw new KeyNotFoundException("Product not found or inactive");
                }
                else if (ex.Message.Contains("Image not found or does not belong to this product"))
                {
                    throw new KeyNotFoundException("Image not found or does not belong to this product");
                }
                else if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new ArgumentException("User not found or inactive");
                }
                
                throw;
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
                this.Logger.LogInformation($"Repository: Delete product image for product {request.ProductId}, image {request.ImageId}, hard: {request.HardDelete}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_DELETE_PRODUCT_IMAGE,
                    request.ProductId,
                    request.ImageId,
                    request.UserId ?? (object)DBNull.Value,
                    request.TenantId ?? (object)DBNull.Value,
                    request.HardDelete,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("Product or image not found");
                }

                // Process deletion result (first result set)
                var deleteRow = result.Tables[0].Rows[0];
                var deleteResponse = new Model.Product.DeleteProductImageResponse
                {
                    ImageId = Convert.ToInt64(deleteRow["ImageId"]),
                    ProductId = Convert.ToInt64(deleteRow["ProductId"]),
                    ImageName = deleteRow["ImageName"]?.ToString() ?? "",
                    WasMain = Convert.ToBoolean(deleteRow["WasMain"]),
                    HardDeleted = Convert.ToBoolean(deleteRow["HardDeleted"]),
                    DeletedAt = Convert.ToDateTime(deleteRow["DeletedAt"]),
                    DeletedBy = deleteRow["DeletedBy"] != DBNull.Value ? Convert.ToInt64(deleteRow["DeletedBy"]) : null,
                    RemainingActiveImages = Convert.ToInt32(deleteRow["RemainingActiveImages"]),
                    Message = deleteRow["Message"]?.ToString() ?? "Image deleted successfully",
                    Success = true
                };

                // Process remaining images (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    deleteResponse.RemainingImages = new List<Model.Product.ProductImageInfo>();
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        deleteResponse.RemainingImages.Add(new Model.Product.ProductImageInfo
                        {
                            ImageId = Convert.ToInt64(row["ImageId"]),
                            ProductId = Convert.ToInt64(row["ProductId"]),
                            Poster = row["Poster"]?.ToString() ?? "",
                            Main = Convert.ToBoolean(row["Main"]),
                            Active = Convert.ToBoolean(row["Active"]),
                            OrderBy = Convert.ToInt32(row["OrderBy"]),
                            Created = Convert.ToDateTime(row["Created"]),
                            Modified = Convert.ToDateTime(row["Modified"])
                        });
                    }
                }

                this.Logger.LogInformation($"Repository: Delete product image successful for product {request.ProductId}, image {request.ImageId}");

                return deleteResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Delete product image error for product {request.ProductId}, image {request.ImageId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Product not found or inactive"))
                {
                    throw new KeyNotFoundException("Product not found or inactive");
                }
                else if (ex.Message.Contains("Image not found or does not belong to this product"))
                {
                    throw new KeyNotFoundException("Image not found or does not belong to this product");
                }
                else if (ex.Message.Contains("Cannot delete the last active main image"))
                {
                    throw new InvalidOperationException("Cannot delete the last active main image. Please add another image first.");
                }
                else if (ex.Message.Contains("Image is already deleted"))
                {
                    throw new InvalidOperationException("Image is already deleted");
                }
                else if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new ArgumentException("User not found or inactive");
                }
                
                throw;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Deserialize JSON string to object or return default if null/empty/invalid
        /// </summary>
        private T DeserializeJsonOrDefault<T>(string json) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
