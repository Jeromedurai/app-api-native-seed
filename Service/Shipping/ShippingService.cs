using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.Query.Model.Shipping;
using Tenant.Query.Repository.Shipping;

namespace Tenant.Query.Service.Shipping
{
    public class ShippingService
    {
        #region Variables
        private readonly ShippingRepository _shippingRepository;
        public ILogger Logger { get; set; }
        #endregion

        #region Constructor
        public ShippingService(
            ShippingRepository shippingRepository,
            ILoggerFactory loggerFactory)
        {
            _shippingRepository = shippingRepository;
            _shippingRepository.Logger = loggerFactory.CreateLogger<ShippingRepository>();
            this.Logger = loggerFactory.CreateLogger<ShippingService>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get all states
        /// </summary>
        public async Task<List<StateResponse>> GetStates(int? tenantId = null, string countryCode = "IN", bool activeOnly = true)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting states for tenant: {tenantId}, country: {countryCode}");
                return await _shippingRepository.GetStates(tenantId, countryCode, activeOnly);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error getting states");
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
                this.Logger.LogInformation($"Service: Calculating shipping for product type: {request.ProductType}, state: {request.StateCode}, courier: {request.CourierType}");

                // Validate request
                if (string.IsNullOrWhiteSpace(request.ProductType))
                {
                    throw new ArgumentException("Product type is required");
                }

                if (request.ProductType != "Seed" && request.ProductType != "Plant")
                {
                    throw new ArgumentException("Product type must be 'Seed' or 'Plant'");
                }

                if (string.IsNullOrWhiteSpace(request.StateCode))
                {
                    throw new ArgumentException("State code is required");
                }

                if (string.IsNullOrWhiteSpace(request.CourierType))
                {
                    throw new ArgumentException("Courier type is required");
                }

                if (request.CourierType != "Postal" && request.CourierType != "Other")
                {
                    throw new ArgumentException("Courier type must be 'Postal' or 'Other'");
                }

                if (request.Subtotal < 0)
                {
                    throw new ArgumentException("Subtotal must be non-negative");
                }

                return await _shippingRepository.CalculateShippingCharge(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error calculating shipping charge");
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
                this.Logger.LogInformation($"Service: Calculating mixed shipping for state: {request.StateCode}, courier: {request.CourierType}");

                // Validate request
                if (string.IsNullOrWhiteSpace(request.StateCode))
                {
                    throw new ArgumentException("State code is required");
                }

                if (string.IsNullOrWhiteSpace(request.CourierType))
                {
                    throw new ArgumentException("Courier type is required");
                }

                if (request.CourierType != "Postal" && request.CourierType != "Other")
                {
                    throw new ArgumentException("Courier type must be 'Postal' or 'Other'");
                }

                if (request.SeedSubtotal < 0 || request.PlantSubtotal < 0 || request.TotalSubtotal < 0)
                {
                    throw new ArgumentException("Subtotals must be non-negative");
                }

                return await _shippingRepository.CalculateMixedShipping(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error calculating mixed shipping");
                throw;
            }
        }

        #region Shipping Rates (Admin CRUD)

        /// <summary>
        /// Get all shipping rate rows for a tenant.
        /// </summary>
        public async Task<List<ShippingRateResponse>> GetShippingRates(int tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting shipping rates for tenant: {tenantId}");
                return await _shippingRepository.GetShippingRates(tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error getting shipping rates");
                throw;
            }
        }

        /// <summary>
        /// Insert or update a shipping rate row after validating its fields.
        /// </summary>
        public async Task<ShippingRateResponse> UpsertShippingRate(UpsertShippingRateRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Upserting shipping rate {request.ShippingRateId} for tenant: {request.TenantId}");

                ValidateRate(request);

                return await _shippingRepository.UpsertShippingRate(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error upserting shipping rate");
                throw;
            }
        }

        /// <summary>
        /// Delete a shipping rate row.
        /// </summary>
        public async Task<int> DeleteShippingRate(long shippingRateId, int tenantId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Deleting shipping rate {shippingRateId} for tenant: {tenantId}");
                return await _shippingRepository.DeleteShippingRate(shippingRateId, tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error deleting shipping rate");
                throw;
            }
        }

        /// <summary>
        /// Activate / deactivate a shipping rate row.
        /// </summary>
        public async Task<int> SetShippingRateActive(long shippingRateId, int tenantId, bool active)
        {
            try
            {
                this.Logger.LogInformation($"Service: Setting shipping rate {shippingRateId} active={active} for tenant: {tenantId}");
                return await _shippingRepository.SetShippingRateActive(shippingRateId, tenantId, active);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error setting shipping rate active state");
                throw;
            }
        }

        // Same guards the calculate endpoints use, so the editor can only persist
        // ProductType/CourierType values the checkout calculation actually supports.
        private static void ValidateRate(UpsertShippingRateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ProductType))
                throw new ArgumentException("Product type is required");

            if (request.ProductType != "Seed" && request.ProductType != "Plant")
                throw new ArgumentException("Product type must be 'Seed' or 'Plant'");

            if (string.IsNullOrWhiteSpace(request.CourierType))
                throw new ArgumentException("Courier type is required");

            if (request.CourierType != "Postal" && request.CourierType != "Other")
                throw new ArgumentException("Courier type must be 'Postal' or 'Other'");

            if (request.BaseCharge < 0 || request.PerUnitCharge < 0 || request.MinCharge < 0)
                throw new ArgumentException("Charges must be non-negative");

            if (request.MaxCharge.HasValue && request.MaxCharge.Value < request.MinCharge)
                throw new ArgumentException("Max charge cannot be less than min charge");

            if (request.FreeShippingThreshold.HasValue && request.FreeShippingThreshold.Value < 0)
                throw new ArgumentException("Free shipping threshold must be non-negative");
        }

        #endregion

        #endregion
    }
}

