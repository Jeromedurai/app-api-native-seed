using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.Query.Model.Address;
using Tenant.Query.Repository.Address;

namespace Tenant.Query.Service.Address
{
    public class AddressService
    {
        #region Variables
        private readonly AddressRepository _addressRepository;
        public ILogger Logger { get; set; }
        #endregion

        #region Constructor
        public AddressService(
            AddressRepository addressRepository,
            ILoggerFactory loggerFactory)
        {
            _addressRepository = addressRepository;
            _addressRepository.Logger = loggerFactory.CreateLogger<AddressRepository>();
            this.Logger = loggerFactory.CreateLogger<AddressService>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get all addresses for a user
        /// </summary>
        public async Task<List<AddressResponse>> GetUserAddresses(long userId, bool activeOnly = true)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting addresses for user: {userId}");
                return await _addressRepository.GetUserAddresses(userId, activeOnly);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error getting user addresses");
                throw;
            }
        }

        /// <summary>
        /// Get address by ID
        /// </summary>
        public async Task<AddressResponse> GetAddressById(long addressId, long? userId = null)
        {
            try
            {
                this.Logger.LogInformation($"Service: Getting address by ID: {addressId}");
                return await _addressRepository.GetAddressById(addressId, userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error getting address by ID");
                throw;
            }
        }

        /// <summary>
        /// Create a new address
        /// </summary>
        public async Task<AddressResponse> CreateAddress(CreateAddressRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Creating address for user: {request.UserId}");

                // Validate request
                if (string.IsNullOrWhiteSpace(request.Street))
                {
                    throw new ArgumentException("Street address is required");
                }

                if (string.IsNullOrWhiteSpace(request.City))
                {
                    throw new ArgumentException("City is required");
                }

                if (string.IsNullOrWhiteSpace(request.State))
                {
                    throw new ArgumentException("State is required");
                }

                if (string.IsNullOrWhiteSpace(request.PostalCode))
                {
                    throw new ArgumentException("Postal code is required");
                }

                // Validate address before creating
                var validationRequest = new ValidateAddressRequest
                {
                    Street = request.Street,
                    City = request.City,
                    State = request.State,
                    PostalCode = request.PostalCode,
                    Country = request.Country
                };

                var validation = await _addressRepository.ValidateAddress(validationRequest);
                if (!validation.IsValid)
                {
                    throw new ArgumentException(validation.ValidationMessage);
                }

                return await _addressRepository.CreateAddress(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error creating address");
                throw;
            }
        }

        /// <summary>
        /// Update an existing address
        /// </summary>
        public async Task<AddressResponse> UpdateAddress(UpdateAddressRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Updating address ID: {request.AddressId}");

                // Validate address if fields are provided
                if (!string.IsNullOrWhiteSpace(request.Street) || 
                    !string.IsNullOrWhiteSpace(request.City) || 
                    !string.IsNullOrWhiteSpace(request.State) || 
                    !string.IsNullOrWhiteSpace(request.PostalCode))
                {
                    var existingAddress = await _addressRepository.GetAddressById(request.AddressId, request.UserId);
                    
                    var validationRequest = new ValidateAddressRequest
                    {
                        Street = request.Street ?? existingAddress.Street,
                        City = request.City ?? existingAddress.City,
                        State = request.State ?? existingAddress.State,
                        PostalCode = request.PostalCode ?? existingAddress.PostalCode,
                        Country = request.Country ?? existingAddress.Country
                    };

                    var validation = await _addressRepository.ValidateAddress(validationRequest);
                    if (!validation.IsValid)
                    {
                        throw new ArgumentException(validation.ValidationMessage);
                    }
                }

                return await _addressRepository.UpdateAddress(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error updating address");
                throw;
            }
        }

        /// <summary>
        /// Delete an address
        /// </summary>
        public async Task<bool> DeleteAddress(long addressId, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Deleting address ID: {addressId} for user: {userId}");
                return await _addressRepository.DeleteAddress(addressId, userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error deleting address");
                throw;
            }
        }

        /// <summary>
        /// Set address as default
        /// </summary>
        public async Task<bool> SetDefaultAddress(long addressId, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Service: Setting address {addressId} as default for user {userId}");
                return await _addressRepository.SetDefaultAddress(addressId, userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error setting default address");
                throw;
            }
        }

        /// <summary>
        /// Admin: Get all addresses with pagination
        /// </summary>
        public async Task<(List<AddressResponse> Addresses, int TotalCount)> AdminGetAllAddresses(GetAddressesRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Admin getting all addresses");
                return await _addressRepository.AdminGetAllAddresses(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error admin getting all addresses");
                throw;
            }
        }

        /// <summary>
        /// Validate address
        /// </summary>
        public async Task<ValidateAddressResponse> ValidateAddress(ValidateAddressRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Service: Validating address");
                return await _addressRepository.ValidateAddress(request);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Service: Error validating address");
                throw;
            }
        }

        #endregion
    }
}

