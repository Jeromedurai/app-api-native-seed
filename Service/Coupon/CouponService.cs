using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.API.Base.Repository;
using Tenant.API.Base.Service;
using Tenant.Query.Model.Coupon;
using Tenant.Query.Repository.Coupon;

namespace Tenant.Query.Service.Coupon
{
    public class CouponService : TnBaseService
    {
        #region Variables
        private readonly CouponRepository _couponRepository;
        #endregion

        #region Constructor
        public CouponService(
            CouponRepository couponRepository,
            ILoggerFactory loggerFactory,
            TnAudit xcAudit,
            TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            _couponRepository = couponRepository;
            _couponRepository.Logger = loggerFactory.CreateLogger<CouponRepository>();
            this.Logger = loggerFactory.CreateLogger<CouponService>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new coupon
        /// </summary>
        public async Task<CouponResponse> CreateCoupon(CreateCouponRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Creating coupon with code: {request.Code}");

                // Validate request
                if (request.Type == "percentage" && request.MaxDiscount == null)
                {
                    throw new ArgumentException("MaxDiscount is required for percentage coupons");
                }

                if (request.EndDate <= request.StartDate)
                {
                    throw new ArgumentException("End date must be after start date");
                }

                if (request.Type == "percentage" && (request.Value < 0 || request.Value > 100))
                {
                    throw new ArgumentException("Percentage value must be between 0 and 100");
                }

                var coupon = await _couponRepository.CreateCoupon(request);
                this.Logger.LogInformation($"Service: Coupon created successfully with ID: {coupon.CouponId}");

                return coupon;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error creating coupon: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Service: Getting all coupons for tenant {tenantId}");

                return await _couponRepository.GetCoupons(tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting coupons: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Service: Getting coupon by ID: {couponId}");

                return await _couponRepository.GetCouponById(couponId, tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting coupon by ID: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Service: Updating coupon ID: {request.CouponId}");

                // Validate if dates are being updated
                if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate <= request.StartDate)
                {
                    throw new ArgumentException("End date must be after start date");
                }

                var coupon = await _couponRepository.UpdateCoupon(request);
                this.Logger.LogInformation($"Service: Coupon updated successfully");

                return coupon;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error updating coupon: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Service: Deleting coupon ID: {couponId}");

                var result = await _couponRepository.DeleteCoupon(couponId, tenantId, deletedBy);
                this.Logger.LogInformation($"Service: Coupon deleted successfully");

                return result;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error deleting coupon: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Validate coupon for use
        /// </summary>
        public async Task<ValidateCouponResponse> ValidateCoupon(ValidateCouponRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Validating coupon code: {request.Code} for amount: {request.Amount}, userId: {request.UserId}, tenantId: {request.TenantId}");

                var response = await _couponRepository.ValidateCoupon(request);
                
                if (response.Valid)
                {
                    this.Logger.LogInformation($"Service: Coupon validated successfully. Discount: {response.DiscountAmount}");
                }
                else
                {
                    this.Logger.LogWarning($"Service: Coupon validation failed: {response.Message}");
                }

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error validating coupon: {ex.Message}", ex);
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
                this.Logger.LogInformation($"Service: Getting usage for coupon ID: {couponId}");

                return await _couponRepository.GetCouponUsage(couponId, tenantId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Service: Error getting coupon usage: {ex.Message}", ex);
                throw;
            }
        }

        #endregion
    }
}

