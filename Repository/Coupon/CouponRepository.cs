using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Coupon;
using Tenant.Query.Model.Constant;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Uitility;

namespace Tenant.Query.Repository.Coupon
{
    public class CouponRepository
    {
        #region Variables
        private readonly DataAccess _dataAccess;
        public ILogger<CouponRepository> Logger { get; set; }
        #endregion

        public CouponRepository(ProductContext dbContext, ILoggerFactory loggerFactory, DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            Logger = loggerFactory.CreateLogger<CouponRepository>();
        }

        #region Coupon CRUD Operations

        /// <summary>
        /// Create a new coupon
        /// </summary>
        public async Task<CouponResponse> CreateCoupon(CreateCouponRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Creating coupon with code: {request.Code}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_CREATE_COUPON,
                    request.Code,
                    request.Type,
                    request.Value,
                    request.MinAmount ?? (object)DBNull.Value,
                    request.MaxDiscount ?? (object)DBNull.Value,
                    request.Description ?? (object)DBNull.Value,
                    request.StartDate,
                    request.EndDate,
                    request.UsageLimit ?? (object)DBNull.Value,
                    request.UsageLimitPerUser ?? (object)DBNull.Value,
                    request.Active,
                    request.TenantId,
                    (object)DBNull.Value // CreatedBy - can be set from context
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new InvalidOperationException("Failed to create coupon");
                }

                return MapToCouponResponse(result.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error creating coupon: {ex.Message}", ex);
                
                // Check for specific error messages from stored procedure RAISERROR
                // Convert to ArgumentException for proper 400 BadRequest responses
                if (ex.Message.Contains("Coupon code already exists for this tenant") ||
                    ex.Message.Contains("already exists"))
                {
                    throw new ArgumentException(ex.Message);
                }
                
                throw;
            }
        }

        /// <summary>
        /// Get all coupons for a tenant
        /// </summary>
        public async Task<List<CouponResponse>> GetCoupons(long tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Getting all coupons for tenant {tenantId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_COUPONS,
                    tenantId
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return new List<CouponResponse>();
                }

                var coupons = result.Tables[0].AsEnumerable()
                    .Select(row => MapToCouponResponse(row))
                    .ToList();

                return coupons;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting coupons: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get coupon by ID
        /// </summary>
        public async Task<CouponResponse> GetCouponById(long couponId, long? tenantId = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Getting coupon by ID: {couponId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_COUPON_BY_ID,
                    couponId,
                    tenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"Coupon with ID {couponId} not found");
                }

                return MapToCouponResponse(result.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting coupon by ID: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Update coupon
        /// </summary>
        public async Task<CouponResponse> UpdateCoupon(UpdateCouponRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Updating coupon ID: {request.CouponId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_UPDATE_COUPON,
                    request.CouponId,
                    request.Code ?? (object)DBNull.Value,
                    request.Type ?? (object)DBNull.Value,
                    request.Value ?? (object)DBNull.Value,
                    request.MinAmount ?? (object)DBNull.Value,
                    request.MaxDiscount ?? (object)DBNull.Value,
                    request.Description ?? (object)DBNull.Value,
                    request.StartDate ?? (object)DBNull.Value,
                    request.EndDate ?? (object)DBNull.Value,
                    request.UsageLimit ?? (object)DBNull.Value,
                    request.UsageLimitPerUser ?? (object)DBNull.Value,
                    request.Active ?? (object)DBNull.Value,
                    request.TenantId,
                    (object)DBNull.Value // UpdatedBy - can be set from context
                ));

                if (result == null || result.Tables.Count == 0)
                {
                    throw new KeyNotFoundException($"Coupon with ID {request.CouponId} not found");
                }

                // Find the table with the SELECT result (should be the last table after UPDATE)
                // UPDATE statements don't return result sets, so we look for the SELECT result
                DataTable resultTable = null;
                for (int i = result.Tables.Count - 1; i >= 0; i--)
                {
                    if (result.Tables[i].Rows.Count > 0 && result.Tables[i].Columns.Contains("CouponId"))
                    {
                        resultTable = result.Tables[i];
                        break;
                    }
                }

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"Coupon with ID {request.CouponId} not found after update");
                }

                return MapToCouponResponse(resultTable.Rows[0]);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error updating coupon: {ex.Message}", ex);
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Column 'UpdatedBy' does not belong to table"))
                {
                    // If UpdatedBy column issue, try to get coupon by ID as fallback
                    this.Logger.LogWarning($"Repository: UpdatedBy column issue, fetching coupon by ID as fallback");
                    try
                    {
                        return await GetCouponById(request.CouponId, request.TenantId);
                    }
                    catch
                    {
                        throw new InvalidOperationException("Failed to update coupon and retrieve updated data");
                    }
                }
                
                throw;
            }
        }

        /// <summary>
        /// Delete coupon (soft delete)
        /// </summary>
        public async Task<bool> DeleteCoupon(long couponId, long tenantId, long? deletedBy = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Deleting coupon ID: {couponId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_DELETE_COUPON,
                    couponId,
                    tenantId,
                    deletedBy ?? (object)DBNull.Value
                ));

                return result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error deleting coupon: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Validate coupon
        /// </summary>
        public async Task<ValidateCouponResponse> ValidateCoupon(ValidateCouponRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Validating coupon code: {request.Code}, amount: {request.Amount}, userId: {request.UserId?.ToString() ?? "NULL"}, tenantId: {request.TenantId}");

                var userIdParam = request.UserId.HasValue ? (object)request.UserId.Value : (object)DBNull.Value;
                this.Logger.LogInformation($"Repository: Calling SP_VALIDATE_COUPON with userId parameter: {(userIdParam == DBNull.Value ? "NULL" : userIdParam.ToString())}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_VALIDATE_COUPON,
                    request.Code,
                    request.Amount,
                    userIdParam,
                    request.TenantId
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return new ValidateCouponResponse
                    {
                        Valid = false,
                        Message = "Invalid coupon code."
                    };
                }

                var row = result.Tables[0].Rows[0];
                return new ValidateCouponResponse
                {
                    Valid = Convert.ToBoolean(row["Valid"]),
                    Message = row["Message"]?.ToString() ?? "",
                    CouponId = row["CouponId"] != DBNull.Value ? Convert.ToInt64(row["CouponId"]) : (long?)null,
                    DiscountAmount = row["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["DiscountAmount"]) : (decimal?)null
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error validating coupon: {ex.Message}", ex);
                
                // Handle CouponUsage table not found error gracefully
                if (ex.Message.Contains("Invalid object name") && ex.Message.Contains("CouponUsage"))
                {
                    this.Logger.LogWarning("CouponUsage table does not exist, continuing validation without per-user limit check");
                    // Return a generic error - the stored procedure should handle this, but just in case
                    return new ValidateCouponResponse
                    {
                        Valid = false,
                        Message = "Error validating coupon. Please try again."
                    };
                }
                
                throw;
            }
        }

        /// <summary>
        /// Get coupon usage statistics
        /// </summary>
        public async Task<(List<CouponUsageResponse> Usages, CouponUsageStatistics Statistics)> GetCouponUsage(long couponId, long? tenantId = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Getting usage for coupon ID: {couponId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_COUPON_USAGE,
                    couponId,
                    tenantId ?? (object)DBNull.Value
                ));

                var usages = new List<CouponUsageResponse>();
                var statistics = new CouponUsageStatistics();

                if (result != null && result.Tables.Count > 0)
                {
                    // Map usage records (first table)
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        usages = result.Tables[0].AsEnumerable()
                            .Where(row => row["UsageId"] != DBNull.Value) // Filter out null rows
                            .Select(row => new CouponUsageResponse
                            {
                                UsageId = row["UsageId"] != DBNull.Value ? Convert.ToInt64(row["UsageId"]) : 0,
                                OrderId = row["OrderId"] != DBNull.Value ? Convert.ToInt64(row["OrderId"]) : 0,
                                UserId = row["UserId"] != DBNull.Value ? Convert.ToInt64(row["UserId"]) : (long?)null,
                                DiscountAmount = row["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["DiscountAmount"]) : 0,
                                OrderAmount = row["OrderAmount"] != DBNull.Value ? Convert.ToDecimal(row["OrderAmount"]) : 0,
                                UsedAt = row["UsedAt"] != DBNull.Value ? Convert.ToDateTime(row["UsedAt"]) : DateTime.UtcNow,
                                OrderNumber = row["OrderNumber"]?.ToString(),
                                UserName = row["UserName"]?.ToString() ?? "Unknown User",
                                UserEmail = row["UserEmail"]?.ToString() ?? ""
                            })
                            .ToList();
                    }

                    // Map statistics (second table)
                    if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                    {
                        var statsRow = result.Tables[1].Rows[0];
                        statistics = new CouponUsageStatistics
                        {
                            TotalUsage = statsRow["TotalUsage"] != DBNull.Value ? Convert.ToInt32(statsRow["TotalUsage"]) : 0,
                            TotalDiscountGiven = statsRow["TotalDiscountGiven"] != DBNull.Value ? Convert.ToDecimal(statsRow["TotalDiscountGiven"]) : 0,
                            TotalOrderAmount = statsRow["TotalOrderAmount"] != DBNull.Value ? Convert.ToDecimal(statsRow["TotalOrderAmount"]) : 0
                        };
                    }
                }

                return (usages, statistics);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error getting coupon usage: {ex.Message}", ex);
                
                // If table doesn't exist, return empty results instead of throwing error
                if (ex.Message.Contains("Invalid object name") && ex.Message.Contains("CouponUsage"))
                {
                    this.Logger.LogWarning("CouponUsage table does not exist, returning empty results");
                    return (new List<CouponUsageResponse>(), new CouponUsageStatistics());
                }
                
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private CouponResponse MapToCouponResponse(DataRow row)
        {
            return new CouponResponse
            {
                CouponId = Convert.ToInt64(row["CouponId"]),
                Code = row["Code"]?.ToString() ?? "",
                Type = row["Type"]?.ToString() ?? "",
                Value = Convert.ToDecimal(row["Value"]),
                MinAmount = row["MinAmount"] != DBNull.Value ? Convert.ToDecimal(row["MinAmount"]) : (decimal?)null,
                MaxDiscount = row["MaxDiscount"] != DBNull.Value ? Convert.ToDecimal(row["MaxDiscount"]) : (decimal?)null,
                Description = row["Description"]?.ToString(),
                StartDate = Convert.ToDateTime(row["StartDate"]),
                EndDate = Convert.ToDateTime(row["EndDate"]),
                UsageLimit = row["UsageLimit"] != DBNull.Value ? Convert.ToInt32(row["UsageLimit"]) : (int?)null,
                UsageLimitPerUser = row["UsageLimitPerUser"] != DBNull.Value ? Convert.ToInt32(row["UsageLimitPerUser"]) : (int?)null,
                UsageCount = row["UsageCount"] != DBNull.Value ? Convert.ToInt32(row["UsageCount"]) : 0,
                Active = Convert.ToBoolean(row["Active"]),
                TenantId = Convert.ToInt64(row["TenantId"]),
                CreatedBy = row["CreatedBy"] != DBNull.Value ? Convert.ToInt64(row["CreatedBy"]) : (long?)null,
                UpdatedBy = row.Table.Columns.Contains("UpdatedBy") && row["UpdatedBy"] != DBNull.Value 
                    ? Convert.ToInt64(row["UpdatedBy"]) 
                    : (long?)null,
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                Status = row.Table.Columns.Contains("Status") && row["Status"] != DBNull.Value 
                    ? row["Status"]?.ToString() 
                    : null
            };
        }

        #endregion
    }
}

