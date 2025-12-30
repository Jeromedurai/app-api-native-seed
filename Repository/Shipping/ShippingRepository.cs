using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Shipping;
using Tenant.Query.Model.Constant;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Uitility;

namespace Tenant.Query.Repository.Shipping
{
    public class ShippingRepository
    {
        #region Variables
        private readonly DataAccess _dataAccess;
        public ILogger<ShippingRepository> Logger { get; set; }
        #endregion

        public ShippingRepository(ProductContext dbContext, ILoggerFactory loggerFactory, DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            Logger = loggerFactory.CreateLogger<ShippingRepository>();
        }

        /// <summary>
        /// Get all states
        /// </summary>
        public async Task<List<StateResponse>> GetStates(int? tenantId = null, string countryCode = "IN", bool activeOnly = true)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Getting states for tenant: {tenantId}, country: {countryCode}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_STATES,
                    tenantId ?? (object)DBNull.Value,
                    countryCode,
                    activeOnly ? 1 : 0
                ));

                var states = new List<StateResponse>();

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        states.Add(new StateResponse
                        {
                            StateId = Convert.ToInt64(row["StateId"]),
                            StateCode = row["StateCode"]?.ToString() ?? "",
                            StateName = row["StateName"]?.ToString() ?? "",
                            CountryCode = row["CountryCode"]?.ToString() ?? "IN",
                            OrderBy = row["OrderBy"] != DBNull.Value ? Convert.ToInt32(row["OrderBy"]) : 0
                        });
                    }
                }
                else
                {
                    this.Logger.LogWarning($"Repository: No states found for tenant: {tenantId}, country: {countryCode}");
                }

                return states;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error getting states: {ex.Message}");
                
                // If States table doesn't exist, return empty list instead of throwing
                if (ex.Message.Contains("Invalid object name") && ex.Message.Contains("States"))
                {
                    this.Logger.LogWarning("States table does not exist, returning empty list");
                    return new List<StateResponse>();
                }
                
                throw;
            }
        }

        /// <summary>
        /// Calculate shipping charge for a single product type
        /// </summary>
        public async Task<CalculateShippingResponse> CalculateShippingCharge(CalculateShippingRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Calculating shipping for product type: {request.ProductType}, state: {request.StateCode}, courier: {request.CourierType}");

                var cmd = _dataAccess.ExecuteNonQueryCMD(
                    Constant.StoredProcedures.SP_CALCULATE_SHIPPING_CHARGE,
                    request.TenantId,
                    request.ProductType,
                    request.StateCode,
                    request.CourierType,
                    request.Subtotal,
                    request.TotalQuantity,
                    DBNull.Value, // @ShippingCharge OUTPUT - auto-discovered by data access layer
                    DBNull.Value  // @FreeShipping OUTPUT - auto-discovered by data access layer
                );

                var shippingCharge = cmd.Parameters["@ShippingCharge"].Value != DBNull.Value 
                    ? Convert.ToDecimal(cmd.Parameters["@ShippingCharge"].Value) 
                    : 0m;
                var freeShipping = cmd.Parameters["@FreeShipping"].Value != DBNull.Value 
                    ? Convert.ToBoolean(cmd.Parameters["@FreeShipping"].Value) 
                    : false;

                return new CalculateShippingResponse
                {
                    ShippingCharge = shippingCharge,
                    FreeShipping = freeShipping,
                    FreeShippingThreshold = 2000, // Default threshold - can be made dynamic later
                    CourierType = request.CourierType
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error calculating shipping charge");
                throw;
            }
        }

        /// <summary>
        /// Calculate shipping charge for mixed cart (seeds + plants)
        /// </summary>
        public async Task<CalculateShippingResponse> CalculateMixedShipping(CalculateMixedShippingRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Calculating mixed shipping for state: {request.StateCode}, courier: {request.CourierType}");

                var cmd = await Task.Run(() => _dataAccess.ExecuteNonQueryCMD(
                    Constant.StoredProcedures.SP_CALCULATE_MIXED_SHIPPING,
                    request.TenantId,
                    request.StateCode,
                    request.CourierType,
                    request.SeedSubtotal,
                    request.PlantSubtotal,
                    request.SeedQuantity,
                    request.PlantQuantity,
                    request.TotalSubtotal,
                    DBNull.Value, // @ShippingCharge OUTPUT - auto-discovered by data access layer
                    DBNull.Value  // @FreeShipping OUTPUT - auto-discovered by data access layer
                ));

                var shippingCharge = cmd.Parameters["@ShippingCharge"].Value != DBNull.Value 
                    ? Convert.ToDecimal(cmd.Parameters["@ShippingCharge"].Value) 
                    : 0m;
                var freeShipping = cmd.Parameters["@FreeShipping"].Value != DBNull.Value 
                    ? Convert.ToBoolean(cmd.Parameters["@FreeShipping"].Value) 
                    : false;

                return new CalculateShippingResponse
                {
                    ShippingCharge = shippingCharge,
                    FreeShipping = freeShipping,
                    FreeShippingThreshold = 2000, // Matches stored procedure threshold
                    CourierType = request.CourierType
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error calculating mixed shipping");
                throw;
            }
        }
    }
}

