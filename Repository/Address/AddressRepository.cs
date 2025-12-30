using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Address;
using Tenant.Query.Model.Constant;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Uitility;

namespace Tenant.Query.Repository.Address
{
    public class AddressRepository
    {
        #region Variables
        private readonly DataAccess _dataAccess;
        public ILogger<AddressRepository> Logger { get; set; }
        #endregion

        public AddressRepository(ProductContext dbContext, ILoggerFactory loggerFactory, DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            Logger = loggerFactory.CreateLogger<AddressRepository>();
        }

        /// <summary>
        /// Get all addresses for a user
        /// </summary>
        public async Task<List<AddressResponse>> GetUserAddresses(long userId, bool activeOnly = true)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Getting addresses for user: {userId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_USER_ADDRESSES,
                    userId,
                    activeOnly ? 1 : 0
                ));

                var addresses = new List<AddressResponse>();

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        addresses.Add(new AddressResponse
                        {
                            AddressId = Convert.ToInt64(row["AddressId"]),
                            UserId = Convert.ToInt64(row["UserId"]),
                            AddressType = row["AddressType"].ToString(),
                            Street = row["Street"].ToString(),
                            City = row["City"].ToString(),
                            State = row["State"].ToString(),
                            PostalCode = row["PostalCode"].ToString(),
                            Country = row["Country"].ToString(),
                            IsDefault = Convert.ToBoolean(row["IsDefault"]),
                            Active = Convert.ToBoolean(row["Active"]),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                        });
                    }
                }

                return addresses;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error getting user addresses");
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
                this.Logger.LogInformation($"Repository: Getting address by ID: {addressId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_ADDRESS_BY_ID,
                    addressId,
                    userId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"Address with ID {addressId} not found");
                }

                var row = result.Tables[0].Rows[0];
                return new AddressResponse
                {
                    AddressId = Convert.ToInt64(row["AddressId"]),
                    UserId = Convert.ToInt64(row["UserId"]),
                    AddressType = row["AddressType"].ToString(),
                    Street = row["Street"].ToString(),
                    City = row["City"].ToString(),
                    State = row["State"].ToString(),
                    PostalCode = row["PostalCode"].ToString(),
                    Country = row["Country"].ToString(),
                    IsDefault = Convert.ToBoolean(row["IsDefault"]),
                    Active = Convert.ToBoolean(row["Active"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error getting address by ID");
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
                this.Logger.LogInformation($"Repository: Creating address for user: {request.UserId}");

                // Execute stored procedure with OUTPUT parameter
                var cmd = _dataAccess.ExecuteNonQueryCMD(
                    Constant.StoredProcedures.SP_CREATE_USER_ADDRESS,
                    request.UserId,
                    request.AddressType ?? "Home",
                    request.Street,
                    request.City,
                    request.State,
                    request.PostalCode,
                    request.Country ?? "IN",
                    request.IsDefault ? 1 : 0,
                    DBNull.Value  // @AddressId OUTPUT - will be auto-discovered
                );

                // Get AddressId from OUTPUT parameter
                var addressId = cmd.Parameters["@AddressId"].Value != DBNull.Value 
                    ? Convert.ToInt64(cmd.Parameters["@AddressId"].Value) 
                    : 0;
                
                if (addressId == 0)
                {
                    throw new Exception("Failed to create address");
                }

                return await GetAddressById(addressId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error creating address");
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
                this.Logger.LogInformation($"Repository: Updating address ID: {request.AddressId}");

                await Task.Run(() => _dataAccess.ExecuteNonQuery(
                    Constant.StoredProcedures.SP_UPDATE_USER_ADDRESS,
                    request.AddressId,
                    request.UserId,
                    request.AddressType ?? (object)DBNull.Value,
                    request.Street ?? (object)DBNull.Value,
                    request.City ?? (object)DBNull.Value,
                    request.State ?? (object)DBNull.Value,
                    request.PostalCode ?? (object)DBNull.Value,
                    request.Country ?? (object)DBNull.Value,
                    request.IsDefault.HasValue ? (object)(request.IsDefault.Value ? 1 : 0) : DBNull.Value,
                    request.Active.HasValue ? (object)(request.Active.Value ? 1 : 0) : DBNull.Value
                ));

                return await GetAddressById(request.AddressId, request.UserId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error updating address");
                throw;
            }
        }

        /// <summary>
        /// Delete an address (soft delete)
        /// </summary>
        public async Task<bool> DeleteAddress(long addressId, long userId)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Deleting address ID: {addressId}");

                await Task.Run(() => _dataAccess.ExecuteNonQuery(
                    Constant.StoredProcedures.SP_DELETE_USER_ADDRESS,
                    addressId,
                    userId
                ));

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error deleting address");
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
                this.Logger.LogInformation($"Repository: Setting address {addressId} as default for user {userId}");

                await Task.Run(() => _dataAccess.ExecuteNonQuery(
                    Constant.StoredProcedures.SP_SET_DEFAULT_ADDRESS,
                    addressId,
                    userId
                ));

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error setting default address");
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
                this.Logger.LogInformation($"Repository: Admin getting all addresses");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_ADMIN_GET_ALL_ADDRESSES,
                    request.TenantId ?? (object)DBNull.Value,
                    request.UserId ?? (object)DBNull.Value,
                    request.ActiveOnly ? 1 : 0,
                    request.PageNumber,
                    request.PageSize
                ));

                var addresses = new List<AddressResponse>();
                int totalCount = 0;

                if (result != null && result.Tables.Count > 0)
                {
                    // First table contains total count
                    if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(result.Tables[0].Rows[0]["TotalCount"]);
                    }

                    // Second table contains addresses
                    if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow row in result.Tables[1].Rows)
                        {
                            addresses.Add(new AddressResponse
                            {
                                AddressId = Convert.ToInt64(row["AddressId"]),
                                UserId = Convert.ToInt64(row["UserId"]),
                                UserName = row["UserName"].ToString(),
                                UserEmail = row["UserEmail"].ToString(),
                                AddressType = row["AddressType"].ToString(),
                                Street = row["Street"].ToString(),
                                City = row["City"].ToString(),
                                State = row["State"].ToString(),
                                PostalCode = row["PostalCode"].ToString(),
                                Country = row["Country"].ToString(),
                                IsDefault = Convert.ToBoolean(row["IsDefault"]),
                                Active = Convert.ToBoolean(row["Active"]),
                                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                            });
                        }
                    }
                }

                return (addresses, totalCount);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error admin getting all addresses");
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
                this.Logger.LogInformation($"Repository: Validating address");

                // Execute stored procedure with OUTPUT parameters
                var cmd = _dataAccess.ExecuteNonQueryCMD(
                    Constant.StoredProcedures.SP_VALIDATE_ADDRESS,
                    request.Street,
                    request.City,
                    request.State,
                    request.PostalCode,
                    request.Country ?? "IN",
                    DBNull.Value, // @IsValid OUTPUT - will be auto-discovered
                    DBNull.Value  // @ValidationMessage OUTPUT - will be auto-discovered
                );

                // Get OUTPUT parameter values
                var isValidValue = cmd.Parameters["@IsValid"].Value;
                var isValid = isValidValue != null && isValidValue != DBNull.Value 
                    ? Convert.ToBoolean(isValidValue) 
                    : false;
                    
                var validationMessageValue = cmd.Parameters["@ValidationMessage"].Value;
                var validationMessage = validationMessageValue != null && validationMessageValue != DBNull.Value 
                    ? validationMessageValue.ToString() 
                    : "Validation failed";

                return new ValidateAddressResponse
                {
                    IsValid = isValid,
                    ValidationMessage = validationMessage
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Repository: Error validating address");
                throw;
            }
        }
    }
}

