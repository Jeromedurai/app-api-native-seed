using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sa.Common.ADO.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Repository;
using Tenant.Query.Context;
using Tenant.Query.Uitility;

namespace Tenant.Query.Repository.User
{
    public class UserRepository : TnBaseQueryRepository<Model.User.User, Context.UserContext>
    {
        DataAccess _dataAccess;
        private string dbConnectionString = string.Empty;
        public UserRepository(UserContext dbContext, ILoggerFactory loggerFactory, DataAccess dataAccess) : base(dbContext, loggerFactory)
        {
            _dataAccess = dataAccess;
            dbConnectionString = dbContext.Database.GetDbConnection().ConnectionString;
        }

        public override Task<Model.User.User> GetById(string tenantId, string id)
        {
            throw new NotImplementedException();
        }

        #region new

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Model.User.SpUserMasterList> GetUser(string spName, long userId)
        {
            try
            {
                //Executing query
                List<Model.User.SpUserMasterList> spUserMasterLists = _dataAccess.ExecuteGenericList<Model.User.SpUserMasterList>(spName,
                    userId);

                return spUserMasterLists;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Authenticate user login
        /// </summary>
        /// <param name="request">Login request</param>
        /// <returns>Login response with user information</returns>
        public async Task<Model.User.LoginResponse> Login(Model.User.LoginRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Login attempt for {request.EmailOrPhone}");

                // Log the parameters being passed to help with debugging
                this.Logger.LogInformation($"Repository: Calling SP_USER_LOGIN with parameters: EmailOrPhone={request.EmailOrPhone}, Password={request.Password?.Substring(0, Math.Min(3, request.Password?.Length ?? 0))}..., RememberMe={request.RememberMe}");

                // Validate parameters before calling stored procedure
                if (string.IsNullOrEmpty(request.EmailOrPhone))
                {
                    throw new ArgumentException("Email or phone number cannot be null or empty");
                }
                if (string.IsNullOrEmpty(request.Password))
                {
                    throw new ArgumentException("Password cannot be null or empty");
                }

                DataSet result;
                try
                {
                    result = await Task.Run(() => _dataAccess.ExecuteDataset(
                        Model.Constant.Constant.StoredProcedures.SP_USER_LOGIN,
                        request.EmailOrPhone,
                        request.Password,
                        request.RememberMe
                    ));
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"Repository: DataAccess.ExecuteDataset error: {ex.Message}");
                    this.Logger.LogError($"Repository: DataAccess.ExecuteDataset error type: {ex.GetType().Name}");
                    throw;
                }
                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new UnauthorizedAccessException("Invalid email/phone or password.");
                }

                var loginResponse = new Model.User.LoginResponse();
                
                // Optimize DataSet processing using LINQ for better performance
                var rows = result.Tables[0].AsEnumerable();
                var firstRow = rows.FirstOrDefault();

                if (firstRow == null)
                {
                    throw new UnauthorizedAccessException("Invalid email/phone or password.");
                }

                // Set user information once from first row
                loginResponse.UserId = firstRow.Field<long?>("UserId") ?? 0;
                loginResponse.FirstName = firstRow.Field<string>("FirstName");
                loginResponse.LastName = firstRow.Field<string>("LastName");
                loginResponse.Email = firstRow.Field<string>("Email");
                loginResponse.Phone = firstRow.Field<string>("Phone");
                loginResponse.Active = firstRow.Field<bool?>("Active") ?? false;
                loginResponse.TenantId = firstRow.Field<long?>("TenantId") ?? 0;
                loginResponse.LastLogin = firstRow.Field<DateTime?>("LastLogin");
                loginResponse.RememberMe = firstRow.Field<bool?>("RememberMe") ?? false;

                // Process roles more efficiently using LINQ
                loginResponse.Roles = rows
                    .Where(r => !r.IsNull("RoleId"))
                    .GroupBy(r => r.Field<long>("RoleId"))
                    .Select(g => g.First())
                    .Select(r => new Model.User.UserRoleInfo
                    {
                        RoleId = r.Field<long>("RoleId"),
                        RoleName = r.Field<string>("RoleName") ?? "",
                        RoleDescription = r.Field<string>("RoleDescription") ?? ""
                    })
                    .ToList();

                // TODO: Generate JWT token if needed
                // loginResponse.Token = GenerateJwtToken(loginResponse);
                // loginResponse.TokenExpiration = DateTime.UtcNow.AddHours(24);

                this.Logger.LogInformation($"Repository: Login successful for user {loginResponse.UserId}");

                return loginResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Login error for {request.EmailOrPhone}: {ex.Message}");
                this.Logger.LogError($"Repository: Login error details - Exception type: {ex.GetType().Name}, Stack trace: {ex.StackTrace}");

                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Account is temporarily locked") ||
                    ex.Message.Contains("Account has been locked"))
                {
                    throw new InvalidOperationException(ex.Message);
                }
                else if (ex.Message.Contains("Invalid email/phone or password"))
                {
                    throw new UnauthorizedAccessException(ex.Message);
                }
                else if (ex.Message.Contains("number of parameters does not match"))
                {
                    this.Logger.LogError($"Repository: Parameter mismatch error - this suggests a stored procedure definition issue");
                    throw new InvalidOperationException($"Stored procedure parameter mismatch: {ex.Message}");
                }

                throw;
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">Registration request</param>
        /// <returns>Registration response with user information and token</returns>
        public async Task<Model.User.RegisterResponse> Register(Model.User.RegisterRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Registration attempt for {request.Email}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_USER_REGISTER,
                    request.Name,
                    request.Email,
                    request.Phone,
                    request.Password,
                    request.TenantId,
                    request.AgreeToTerms
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to register user");
                }

                var registerResponse = new Model.User.RegisterResponse();
                var registeredUser = new Model.User.RegisteredUser();

                // Optimize DataSet processing using LINQ for better performance
                var rows = result.Tables[0].AsEnumerable();
                var firstRow = rows.FirstOrDefault();

                if (firstRow == null)
                {
                    throw new Exception("Failed to register user");
                }

                // Set user information once from first row
                registeredUser.UserId = firstRow.Field<long>("UserId");
                registeredUser.FirstName = firstRow.Field<string>("FirstName");
                registeredUser.LastName = firstRow.Field<string>("LastName");
                registeredUser.Email = firstRow.Field<string>("Email");
                registeredUser.Phone = firstRow.Field<string>("Phone");
                registeredUser.Active = firstRow.Field<bool>("Active");
                registeredUser.TenantId = firstRow.Field<long>("TenantId");
                registeredUser.EmailVerified = firstRow.Field<bool>("EmailVerified");
                registeredUser.PhoneVerified = firstRow.Field<bool>("PhoneVerified");
                registeredUser.CreatedAt = firstRow.Field<DateTime>("CreatedAt");

                // Process roles more efficiently using LINQ
                registeredUser.Roles = rows
                    .Where(r => !r.IsNull("RoleId"))
                    .GroupBy(r => r.Field<long>("RoleId"))
                    .Select(g => g.First())
                    .Select(r => new Model.User.UserRoleInfo
                    {
                        RoleId = r.Field<long>("RoleId"),
                        RoleName = r.Field<string>("RoleName"),
                        RoleDescription = r.Field<string>("RoleDescription")
                    })
                    .ToList();
                registerResponse.User = registeredUser;

                // Generate JWT token and refresh token
                var tokenExpiration = DateTime.UtcNow.AddHours(24); // 24 hours
                registerResponse.Token = GenerateJwtToken(registeredUser, tokenExpiration);
                registerResponse.RefreshToken = GenerateRefreshToken();
                registerResponse.ExpiresIn = 3600; // 1 hour in seconds
                registerResponse.TokenExpiration = tokenExpiration;

                this.Logger.LogInformation($"Repository: Registration successful for user {registeredUser.UserId}");

                return registerResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Registration error for {request.Email}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Email address is already registered") ||
                    ex.Message.Contains("Phone number is already registered"))
                {
                    throw new InvalidOperationException(ex.Message);
                }
                else if (ex.Message.Contains("You must agree to the terms and conditions"))
                {
                    throw new ArgumentException(ex.Message);
                }
                
                throw;
            }
        }

        /// <summary>
        /// Generate JWT token for user (placeholder implementation)
        /// </summary>
        /// <param name="user">User information</param>
        /// <param name="expiration">Token expiration</param>
        /// <returns>JWT token string</returns>
        private string GenerateJwtToken(Model.User.RegisteredUser user, DateTime expiration)
        {
            // TODO: Implement actual JWT token generation
            // This is a placeholder - implement with proper JWT library
            var tokenPayload = $"{{\"userId\":{user.UserId},\"email\":\"{user.Email}\",\"tenantId\":{user.TenantId},\"exp\":{((DateTimeOffset)expiration).ToUnixTimeSeconds()}}}";
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenPayload);
            return $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{Convert.ToBase64String(tokenBytes)}.signature_placeholder";
        }

        /// <summary>
        /// Generate refresh token (placeholder implementation)
        /// </summary>
        /// <returns>Refresh token string</returns>
        private string GenerateRefreshToken()
        {
            // TODO: Implement actual refresh token generation
            // This is a placeholder - implement with proper cryptographic randomness
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Logout user and invalidate tokens
        /// </summary>
        /// <param name="request">Logout request</param>
        /// <returns>Logout confirmation message</returns>
        public async Task<string> Logout(Model.User.LogoutRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Logout attempt for user {request.UserId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_USER_LOGOUT,
                    request.UserId,
                    request.DeviceId ?? (object)DBNull.Value,
                    request.LogoutFromAllDevices
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }

                var logoutResult = result.Tables[0].Rows[0];
                var message = logoutResult["Message"]?.ToString() ?? "Logged out successfully";
                var logoutTime = Convert.ToDateTime(logoutResult["LogoutTime"]);
                var logoutFromAllDevices = Convert.ToBoolean(logoutResult["LogoutFromAllDevices"]);

                this.Logger.LogInformation($"Repository: Logout successful for user {request.UserId} at {logoutTime}");

                // Log additional activity if needed
                await LogUserActivity(request, "LOGOUT_SUCCESS", message);

                return message;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Logout error for user {request.UserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException(ex.Message);
                }
                
                // Log failed activity
                await LogUserActivity(request, "LOGOUT_FAILED", ex.Message);
                
                throw;
            }
        }

        /// <summary>
        /// Log user activity for logout operations
        /// </summary>
        /// <param name="request">Logout request</param>
        /// <param name="activityType">Type of activity</param>
        /// <param name="description">Activity description</param>
        /// <returns>Task</returns>
        private async Task LogUserActivity(Model.User.LogoutRequest request, string activityType, string description)
        {
            try
            {
                // Note: This assumes you have a stored procedure for logging activities
                // If not, you can implement direct SQL insert or create the stored procedure
                // _dataAccess.ExecuteNonQuery("SP_LOG_USER_ACTIVITY", 
                //     request.UserId, activityType, description, 
                //     request.IpAddress ?? (object)DBNull.Value,
                //     request.UserAgent ?? (object)DBNull.Value,
                //     request.DeviceId ?? (object)DBNull.Value,
                //     DateTime.UtcNow);
                
                this.Logger.LogInformation($"User activity logged: {activityType} for user {request.UserId}");
            }
            catch (Exception ex)
            {
                // Don't throw here as this is auxiliary logging
                this.Logger.LogWarning($"Failed to log user activity: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user profile information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Optional tenant ID for validation</param>
        /// <returns>User profile data</returns>
        public async Task<Model.User.UserProfileData> GetUserProfile(long userId, long? tenantId = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Get profile for user {userId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_GET_USER_PROFILE,
                    userId,
                    tenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }

                var profileData = new Model.User.UserProfileData();
                var userRoles = new Dictionary<long, Model.User.UserRoleInfo>();
                var userAddresses = new Dictionary<long, Model.User.UserAddressInfo>();
                var userPreferences = new Dictionary<string, Model.User.UserPreferenceInfo>();

                // Process main profile data and related information
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    // Set user basic information (only once)
                    if (profileData.UserId == 0)
                    {
                        profileData.UserId = Convert.ToInt64(row["UserId"]);
                        profileData.FirstName = row["FirstName"]?.ToString();
                        profileData.LastName = row["LastName"]?.ToString();
                        profileData.Email = row["Email"]?.ToString();
                        profileData.Phone = row["Phone"]?.ToString();
                        profileData.Active = Convert.ToBoolean(row["Active"]);
                        profileData.TenantId = Convert.ToInt64(row["TenantId"]);
                        profileData.EmailVerified = Convert.ToBoolean(row["EmailVerified"]);
                        profileData.PhoneVerified = Convert.ToBoolean(row["PhoneVerified"]);
                        profileData.CreatedAt = Convert.ToDateTime(row["CreatedAt"]);
                        profileData.UpdatedAt = row["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedAt"]) : (DateTime?)null;
                        profileData.LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : (DateTime?)null;
                        profileData.LastLogout = row["LastLogout"] != DBNull.Value ? Convert.ToDateTime(row["LastLogout"]) : (DateTime?)null;
                        profileData.ProfilePicture = row["ProfilePicture"]?.ToString();
                        profileData.DateOfBirth = row["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(row["DateOfBirth"]) : (DateTime?)null;
                        profileData.Gender = row["Gender"]?.ToString();
                        profileData.Timezone = row["Timezone"]?.ToString();
                        profileData.Language = row["Language"]?.ToString();
                        profileData.Country = row["Country"]?.ToString();
                        profileData.City = row["City"]?.ToString();
                        profileData.State = row["State"]?.ToString();
                        profileData.PostalCode = row["PostalCode"]?.ToString();
                        profileData.Bio = row["Bio"]?.ToString();
                        profileData.Website = row["Website"]?.ToString();
                        profileData.CompanyName = row["CompanyName"]?.ToString();
                        profileData.JobTitle = row["JobTitle"]?.ToString();
                        profileData.PreferredContactMethod = row["PreferredContactMethod"]?.ToString();
                        profileData.NotificationSettings = row["NotificationSettings"]?.ToString();
                        profileData.PrivacySettings = row["PrivacySettings"]?.ToString();
                    }

                    // Add role if it exists and hasn't been added yet
                    if (row["RoleId"] != DBNull.Value)
                    {
                        var roleId = Convert.ToInt64(row["RoleId"]);
                        if (!userRoles.ContainsKey(roleId))
                        {
                            userRoles[roleId] = new Model.User.UserRoleInfo
                            {
                                RoleId = roleId,
                                RoleName = row["RoleName"]?.ToString(),
                                RoleDescription = row["RoleDescription"]?.ToString()
                            };
                        }
                    }

                    // Add address if it exists and hasn't been added yet
                    if (row["AddressId"] != DBNull.Value)
                    {
                        var addressId = Convert.ToInt64(row["AddressId"]);
                        if (!userAddresses.ContainsKey(addressId))
                        {
                            userAddresses[addressId] = new Model.User.UserAddressInfo
                            {
                                AddressId = addressId,
                                AddressType = row["AddressType"]?.ToString(),
                                Street = row["Street"]?.ToString(),
                                City = row["AddressCity"]?.ToString(),
                                State = row["AddressState"]?.ToString(),
                                PostalCode = row["AddressPostalCode"]?.ToString(),
                                Country = row["AddressCountry"]?.ToString(),
                                IsDefault = Convert.ToBoolean(row["IsDefaultAddress"])
                            };
                        }
                    }

                    // Add preference if it exists and hasn't been added yet
                    // if (row["PreferenceKey"] != DBNull.Value)
                    // {
                    //     var preferenceKey = row["PreferenceKey"]?.ToString();
                    //     if (!string.IsNullOrEmpty(preferenceKey) && !userPreferences.ContainsKey(preferenceKey))
                    //     {
                    //         userPreferences[preferenceKey] = new Model.User.UserPreferenceInfo
                    //         {
                    //             Key = preferenceKey,
                    //             Value = row["PreferenceValue"]?.ToString(),
                    //             Type = row["PreferenceType"]?.ToString()
                    //         };
                    //     }
                    // }
                }

                // Set collections
                profileData.Roles = userRoles.Values.ToList();
                profileData.Addresses = userAddresses.Values.ToList();
                profileData.Preferences = userPreferences.Values.ToList();

                // Process statistics if available (second result set)
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow statRow in result.Tables[1].Rows)
                    {
                        var statType = statRow["StatType"]?.ToString();
                        var statValue = Convert.ToInt32(statRow["StatValue"]);

                        switch (statType)
                        {
                            case "LOGIN_COUNT":
                                profileData.Statistics.LoginCount = statValue;
                                break;
                            case "LAST_ACTIVITY":
                                profileData.Statistics.DaysSinceLastActivity = statValue;
                                break;
                            case "PROFILE_COMPLETION":
                                profileData.Statistics.ProfileCompletion = statValue;
                                break;
                        }
                    }
                }

                this.Logger.LogInformation($"Repository: Profile retrieval successful for user {userId}");

                return profileData;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Profile retrieval error for user {userId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found") || ex.Message.Contains("inactive"))
                {
                    throw new KeyNotFoundException(ex.Message);
                }
                
                throw;
            }
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        /// <param name="request">Profile update request</param>
        /// <returns>Success confirmation message</returns>
        public async Task<string> UpdateProfile(Model.User.UpdateProfileRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Profile update for user {request.UserId}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_UPDATE_USER_PROFILE,
                    request.UserId,
                    request.Name ?? (object)DBNull.Value,
                    request.Phone ?? (object)DBNull.Value,
                    request.DateOfBirth ?? (object)DBNull.Value,
                    request.Gender ?? (object)DBNull.Value,
                    request.Bio ?? (object)DBNull.Value,
                    request.Website ?? (object)DBNull.Value,
                    request.CompanyName ?? (object)DBNull.Value,
                    request.JobTitle ?? (object)DBNull.Value,
                    request.Country ?? (object)DBNull.Value,
                    request.City ?? (object)DBNull.Value,
                    request.State ?? (object)DBNull.Value,
                    request.PostalCode ?? (object)DBNull.Value,
                    request.Timezone ?? (object)DBNull.Value,
                    request.Language ?? (object)DBNull.Value,
                    request.PreferredContactMethod ?? (object)DBNull.Value,
                    // Address parameters
                    request.Address?.Street ?? (object)DBNull.Value,
                    request.Address?.City ?? (object)DBNull.Value,
                    request.Address?.State ?? (object)DBNull.Value,
                    request.Address?.ZipCode ?? (object)DBNull.Value,
                    request.Address?.Country ?? (object)DBNull.Value,
                    request.Address?.AddressType ?? "Home",
                    request.Address?.UpdateIfExists ?? true
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new KeyNotFoundException("User not found or inactive");
                }

                var updateResult = result.Tables[0].Rows[0];
                var message = updateResult["Message"]?.ToString() ?? "Profile updated successfully";
                var updateTime = Convert.ToDateTime(updateResult["UpdatedAt"]);

                this.Logger.LogInformation($"Repository: Profile update successful for user {request.UserId} at {updateTime}");

                return message;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Profile update error for user {request.UserId}: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or inactive"))
                {
                    throw new KeyNotFoundException(ex.Message);
                }
                else if (ex.Message.Contains("Phone number is already registered"))
                {
                    throw new InvalidOperationException(ex.Message);
                }
                
                throw;
            }
        }

        /// <summary>
        /// Reset user password using reset token
        /// </summary>
        /// <param name="request">Password reset request</param>    
        /// <returns>Password reset response</returns>
        public async Task<Model.User.ResetPasswordResponse> ResetPassword(Model.User.ResetPasswordRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Password reset attempt for token: {request.UserId}...");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_RESET_PASSWORD,
                    request.UserId,
                    request.NewPassword,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new UnauthorizedAccessException("Invalid or expired reset token");
                }

                var resetResult = result.Tables[0].Rows[0];
                var resetResponse = new Model.User.ResetPasswordResponse
                {
                    UserId = Convert.ToInt64(resetResult["UserId"]),
                    Email = resetResult["Email"]?.ToString() ?? "",
                    Message = resetResult["Message"]?.ToString() ?? "Password reset successfully",
                    ResetDate = Convert.ToDateTime(resetResult["ResetDate"]),
                    Success = true
                };

                this.Logger.LogInformation($"Repository: Password reset successful for user {resetResponse.UserId}");

                return resetResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Password reset error for token {request.UserId}...: {ex.Message}");
                
                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("Invalid or expired reset token"))
                {
                    throw new UnauthorizedAccessException("Invalid or expired reset token");
                }
                else if (ex.Message.Contains("Reset token has already been used"))
                {
                    throw new InvalidOperationException("Reset token has already been used");
                }
                else if (ex.Message.Contains("Reset token has expired"))
                {
                    throw new UnauthorizedAccessException("Reset token has expired. Please request a new password reset");
                }
                
                throw;
            }
        }

        #endregion

        #region old
        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="locationId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Model.User.User> GetById(string tenantId, string locationId, string id)
        {
            try
            {
                //Logger
                this.Logger.LogInformation($"Calling GetById({tenantId}, {locationId}, {id})");

                //retrive user      
                Model.User.User user = await this.DbContext.Users
                                                    .Where(x => x.UserId.Equals(id) &&
                                                     x.TenantId.Equals(tenantId)).FirstOrDefaultAsync();

                //Logger
                this.Logger.LogInformation($"Called GetById({tenantId}, {locationId}, {id})");

                //return 
                return user;
            }
            catch (Exception ex)
            {
                //Error logger
                this.Logger.LogError($"GetUser Error({ex.Message}) : {ex.InnerException}");
                throw ex;
            }
        }

        /// <summary>
        /// Get Role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        internal async Task<List<Model.User.Role>> GetRoles(string[] roleId)
        {
            try
            {
                //Logger
                this.Logger.LogInformation($"Calling GetRoles");

                //Query build 
                IQueryable<Model.User.Role> query = this.DbContext.Roles.AsQueryable();


                if (roleId != null && roleId.Count() > 0)
                {
                    //get role list by give id's
                    query = query.Where(x => roleId.Contains(x.Guid));
                }

                //Execute Query
                List<Model.User.Role> roles = await query.ToListAsync();

                //Logger 
                this.Logger.LogInformation($"Called GetRoles");

                //return
                return roles;
            }
            catch (Exception ex)
            {
                //Error logger
                this.Logger.LogError($"GetUser Error({ex.Message}) : {ex.InnerException}");

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
                    request.Status ?? (object)DBNull.Value,
                    request.Role ?? (object)DBNull.Value
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
                        var firstName = row["FirstName"]?.ToString() ?? "";
                        var lastName = row["LastName"]?.ToString() ?? "";
                        var fullName = $"{firstName} {lastName}".Trim();

                        var user = new Model.Admin.AdminUserInfo
                        {
                            UserId = Convert.ToInt64(row["UserId"]),
                            FirstName = firstName,
                            LastName = lastName,
                            FullName = fullName,
                            Email = row["Email"]?.ToString() ?? "",
                            Phone = row["Phone"]?.ToString() ?? "",
                            Role = row["Role"]?.ToString() ?? "",
                            RoleId = row["RoleId"] != DBNull.Value ? Convert.ToInt64(row["RoleId"]) : null,
                            EmailVerified = row["EmailVerified"] != DBNull.Value ? Convert.ToBoolean(row["EmailVerified"]) : false,
                            PhoneVerified = row["PhoneVerified"] != DBNull.Value ? Convert.ToBoolean(row["PhoneVerified"]) : false,
                            Status = row["Status"]?.ToString() ?? "active",
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : null,
                            TenantId = row["TenantId"] != DBNull.Value ? Convert.ToInt64(row["TenantId"]) : null,
                            Avatar = row["Avatar"]?.ToString() ?? "",
                            Active = row["Active"] != DBNull.Value ? Convert.ToBoolean(row["Active"]) : true
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
        /// Update user status (Admin only)
        /// </summary>
        /// <param name="request">Update user status request</param>
        /// <returns>Success message</returns>
        public async Task<string> UpdateUserStatus(Model.Admin.UpdateUserStatusRequest request)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Admin update user status by admin {request.AdminUserId} for user {request.UserId} to {request.Status}");

                // Use _dataAccess.ExecuteScalar wrapped in Task.Run like ProductRepository pattern
                var result = await Task.Run(() => _dataAccess.ExecuteScalar(
                    Model.Constant.Constant.StoredProcedures.SP_ADMIN_UPDATE_USER_STATUS,
                    request.AdminUserId,
                    request.UserId,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Status,
                    request.Reason ?? (object)DBNull.Value,
                    request.IpAddress ?? (object)DBNull.Value,
                    request.UserAgent ?? (object)DBNull.Value
                ));

                if (result == null)
                {
                    throw new UnauthorizedAccessException("Admin user not found or insufficient privileges");
                }

                var message = result?.ToString() ?? "User updated successfully";
                this.Logger.LogInformation($"Repository: Admin update user status successful by admin {request.AdminUserId} for user {request.UserId}");

                return message;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Admin update user status error by admin {request.AdminUserId}: {ex.Message}");

                // Check for specific error messages from stored procedure
                if (ex.Message.Contains("User not found or insufficient privileges"))
                {
                    throw new UnauthorizedAccessException("User not found or insufficient privileges");
                }

                if (ex.Message.Contains("not found") || ex.Message.Contains("does not exist"))
                {
                    throw new KeyNotFoundException("User not found");
                }

                throw;
            }
        }

        /// <summary>
        /// Request password reset OTP
        /// </summary>
        public async Task<Model.User.ForgotPasswordResponse> RequestPasswordReset(Model.User.ForgotPasswordRequest request, string ipAddress = null, string userAgent = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Password reset requested for email: {request.Email}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_REQUEST_PASSWORD_RESET,
                    request.Email,
                    ipAddress ?? (object)DBNull.Value,
                    userAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to process password reset request");
                }

                var row = result.Tables[0].Rows[0];
                var response = new Model.User.ForgotPasswordResponse
                {
                    Message = GetColumnValue<string>(row, "Message", "If email exists, OTP has been sent to your email address."),
                    ExpiresInSeconds = GetColumnValue<int>(row, "ExpiresInSeconds", 600),
                    UserId = GetColumnValue<long>(row, "UserId", 0),
                    OTP = GetColumnValue<string>(row, "OTP", ""),
                    Email = GetColumnValue<string>(row, "Email", request.Email),
                    FirstName = GetColumnValue<string>(row, "FirstName", "")
                };

                this.Logger.LogInformation($"Repository: Password reset OTP generated for user: {response.UserId}");

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error requesting password reset for {request.Email}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verify OTP and reset password
        /// </summary>
        public async Task<Model.User.ResetPasswordWithOtpResponse> ResetPasswordWithOtp(Model.User.ResetPasswordWithOtpRequest request, string ipAddress = null, string userAgent = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Password reset with OTP for email: {request.Email}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_VERIFY_RESET_OTP,
                    request.Email,
                    request.OTP,
                    request.NewPassword,
                    ipAddress ?? (object)DBNull.Value,
                    userAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to reset password");
                }

                var row = result.Tables[0].Rows[0];
                var response = new Model.User.ResetPasswordWithOtpResponse
                {
                    UserId = GetColumnValue<long>(row, "UserId", 0),
                    Message = GetColumnValue<string>(row, "Message", "Password reset successful"),
                    ResetAt = GetColumnValue<DateTime>(row, "ResetAt", DateTime.UtcNow)
                };

                this.Logger.LogInformation($"Repository: Password reset successful for user: {response.UserId}");

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error resetting password for {request.Email}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Resend password reset OTP
        /// </summary>
        public async Task<Model.User.ForgotPasswordResponse> ResendResetOtp(Model.User.ForgotPasswordRequest request, string ipAddress = null, string userAgent = null)
        {
            try
            {
                this.Logger.LogInformation($"Repository: Resending password reset OTP for email: {request.Email}");

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Model.Constant.Constant.StoredProcedures.SP_RESEND_RESET_OTP,
                    request.Email,
                    ipAddress ?? (object)DBNull.Value,
                    userAgent ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Failed to resend password reset OTP");
                }

                var row = result.Tables[0].Rows[0];
                var response = new Model.User.ForgotPasswordResponse
                {
                    Message = GetColumnValue<string>(row, "Message", "If email exists, OTP has been sent to your email address."),
                    ExpiresInSeconds = GetColumnValue<int>(row, "ExpiresInSeconds", 600),
                    UserId = GetColumnValue<long>(row, "UserId", 0),
                    OTP = GetColumnValue<string>(row, "OTP", ""),
                    Email = GetColumnValue<string>(row, "Email", request.Email),
                    FirstName = GetColumnValue<string>(row, "FirstName", "")
                };

                this.Logger.LogInformation($"Repository: Password reset OTP resent for user: {response.UserId}");

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Repository: Error resending password reset OTP for {request.Email}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Helper method to get column value with default
        /// </summary>
        private static T GetColumnValue<T>(DataRow row, string columnName, T defaultValue = default)
        {
            if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                var targetType = typeof(T);
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }
                return (T)Convert.ChangeType(row[columnName], targetType);
            }
            return defaultValue;
        }

        #endregion
    }
}
