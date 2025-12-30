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

        #endregion
    }
}

