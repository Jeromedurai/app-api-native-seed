using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Repository;
using Tenant.API.Base.Service;
using Tenant.API.Base.Model;

namespace Tenant.Query.Service.User
{
    public class UserService : TnBaseService
    {
        #region Variables

        private Repository.User.UserRepository UserRepository;

        #endregion

        public UserService(Repository.User.UserRepository userRepository, ILoggerFactory loggerFactory, TnAudit xcAudit, TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            this.UserRepository = userRepository;
            this.UserRepository.Logger = loggerFactory.CreateLogger<Repository.User.UserRepository>();
        }

        #region new
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="tenantId">Tenant identifier.</param>
        public List<Model.User.UserDetails> GetUserWithAddress(long userId)
        {
            try
            {
                List<Model.User.UserDetails> users = new List<Model.User.UserDetails>();

                // Fetch user details
                List<Model.User.SpUserMasterList> spUserMasterList = this.UserRepository.GetUser("SP_CUSTOMER", userId);

                // Map SpUserMasterList to UserDetails and include address details
                if (spUserMasterList != null)
                {
                    users.AddRange(spUserMasterList.Select(spUser => new Model.User.UserDetails
                    {
                        UserId = spUser.UserId,
                        FirstName = spUser.FirstName,
                        LastName = spUser.LastName,
                        Email = spUser.Email,
                        Phone = spUser.Phone,
                        SystemAdmin=spUser.SystemAdmin,
                        CreatedAt = spUser.CreatedAt,
                        Addresses = new List<Model.User.Address>
                        {
                            new Model.User.Address
                            {
                                AddressType = spUser.AddressType,
                                Street = spUser.Street,
                                City = spUser.City,
                                State = spUser.State,
                                PostalCode = spUser.PostalCode,
                                Country = spUser.Country,
                                IsDefault = spUser.IsDefault
                            }
                        }
                    }));
                }

                return users;
            }
            catch (System.Exception ex)
            {
                throw ex;
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
                this.Logger.LogInformation($"Login attempt for: {request.EmailOrPhone}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrWhiteSpace(request.EmailOrPhone))
                    throw new ArgumentException("Email or phone is required");

                if (string.IsNullOrWhiteSpace(request.Password))
                    throw new ArgumentException("Password is required");

                // Call repository to authenticate user
                var loginResponse = await this.UserRepository.Login(request);

                this.Logger.LogInformation($"Login successful for user: {loginResponse.UserId}");

                return loginResponse;
            }
            catch (UnauthorizedAccessException)
            {
                this.Logger.LogWarning($"Invalid login attempt for: {request?.EmailOrPhone}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this.Logger.LogWarning($"Account locked for: {request?.EmailOrPhone}");
                throw;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"Login Error for {request?.EmailOrPhone}: {ex.Message}");
                throw new System.Exception("An error occurred during login.", ex);
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
                this.Logger.LogInformation($"Registration attempt for: {request.Email}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Name is required");

                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ArgumentException("Email is required");

                if (string.IsNullOrWhiteSpace(request.Phone))
                    throw new ArgumentException("Phone is required");

                if (string.IsNullOrWhiteSpace(request.Password))
                    throw new ArgumentException("Password is required");

                if (request.Password != request.ConfirmPassword)
                    throw new ArgumentException("Passwords do not match");

                if (!request.AgreeToTerms)
                    throw new ArgumentException("You must agree to the terms and conditions");

                // Call repository to register user
                var registerResponse = await this.UserRepository.Register(request);

                this.Logger.LogInformation($"Registration successful for user: {registerResponse.User.UserId}");

                return registerResponse;
            }
            catch (InvalidOperationException)
            {
                this.Logger.LogWarning($"Registration failed - email or phone already exists: {request?.Email}");
                throw;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"Registration Error for {request?.Email}: {ex.Message}");
                throw new System.Exception("An error occurred during registration.", ex);
            }
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
                this.Logger.LogInformation($"Logout attempt for user: {request.UserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                // Call repository to logout user
                var logoutMessage = await this.UserRepository.Logout(request);

                this.Logger.LogInformation($"Logout successful for user: {request.UserId}");

                return logoutMessage;
            }
            catch (KeyNotFoundException)
            {
                this.Logger.LogWarning($"Logout failed - user not found: {request?.UserId}");
                throw;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"Logout Error for user {request?.UserId}: {ex.Message}");
                throw new System.Exception("An error occurred during logout.", ex);
            }
        }

        /// <summary>
        /// Get user profile information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Optional tenant ID for validation</param>
        /// <returns>User profile response</returns>
        public async Task<Model.User.UserProfileData> GetUserProfile(long userId, long? tenantId = null)
        {
            try
            {
                this.Logger.LogInformation($"Get profile attempt for user: {userId}");

                if (userId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                // Call repository to get user profile
                var profileData = await this.UserRepository.GetUserProfile(userId, tenantId);

                this.Logger.LogInformation($"Profile retrieval successful for user: {userId}");

                return profileData;
            }
            catch (KeyNotFoundException)
            {
                this.Logger.LogWarning($"Profile retrieval failed - user not found: {userId}");
                throw;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"Profile retrieval error for user {userId}: {ex.Message}");
                throw new System.Exception("An error occurred while retrieving the user profile.", ex);
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
                this.Logger.LogInformation($"Profile update attempt for user: {request.UserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                // Validate date of birth if provided
                if (request.DateOfBirth.HasValue && request.DateOfBirth.Value > DateTime.Now)
                    throw new ArgumentException("Date of birth cannot be in the future");

                // Validate age if date of birth is provided (must be at least 13 years old)
                if (request.DateOfBirth.HasValue)
                {
                    var age = DateTime.Now.Year - request.DateOfBirth.Value.Year;
                    if (DateTime.Now < request.DateOfBirth.Value.AddYears(age))
                        age--;
                    
                    if (age < 13)
                        throw new ArgumentException("User must be at least 13 years old");
                }

                // Call repository to update profile
                var updateMessage = await this.UserRepository.UpdateProfile(request);

                this.Logger.LogInformation($"Profile update successful for user: {request.UserId}");

                return updateMessage;
            }
            catch (KeyNotFoundException)
            {
                this.Logger.LogWarning($"Profile update failed - user not found: {request?.UserId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this.Logger.LogWarning($"Profile update failed - phone already exists: {request?.Phone}");
                throw;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"Profile update error for user {request?.UserId}: {ex.Message}");
                throw new System.Exception("An error occurred while updating the profile.", ex);
            }
        }

        /// <summary>
        /// Reset user password using reset token
        /// </summary>
        /// <param name="request">Password reset request</param>
        /// <returns>Success confirmation message</returns>
        public async Task<string> ResetPassword(Model.User.ResetPasswordRequest request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    throw new ArgumentException("New password is required");

                if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
                    throw new ArgumentException("Password confirmation is required");

                if (request.NewPassword != request.ConfirmPassword)
                    throw new ArgumentException("Password and confirmation password do not match");

                // Additional password strength validation (complementing the regex validation)
                if (request.NewPassword.Length < 8)
                    throw new ArgumentException("Password must be at least 8 characters long");

                if (request.NewPassword.Length > 255)
                    throw new ArgumentException("Password cannot exceed 255 characters");

                // Check for common weak passwords
                var commonWeakPasswords = new[] 
                {
                    "password", "password123", "123456789", "qwerty123", "admin123",
                    "welcome123", "letmein123", "password1", "123456", "qwerty"
                };

                if (commonWeakPasswords.Any(weak => request.NewPassword.ToLower().Contains(weak.ToLower())))
                    throw new ArgumentException("Password contains common weak patterns. Please choose a stronger password");

                // Call repository to reset password
                var resetResponse = await this.UserRepository.ResetPassword(request);

                this.Logger.LogInformation($"Password reset successful for user: {resetResponse.UserId}");

                return resetResponse.Message;
            }
            catch (UnauthorizedAccessException)
            {
                this.Logger.LogWarning($"Invalid or expired reset token used: {request?.UserId}");
                throw;
            }
            catch (InvalidOperationException)
            {
                this.Logger.LogWarning($"Reset token already used: {request?.UserId}");
                throw;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"Password reset error for token {request?.UserId}");
                throw new System.Exception("An error occurred while resetting the password.", ex);
            }
        }
        #endregion

        #region old



        /// <summary>
        /// Get list of roles
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        internal async Task<List<Model.User.Role>> GetRoles(string[] roleId)
        {
            try
            {
                //Logger 
                this.Logger.LogInformation($"Calling GetRoles");

                //Get roles asynchronously
                var roles = await this.UserRepository.GetRoles(roleId);

                //Logger
                this.Logger.LogInformation($"Called GetRoles");

                //return 
                return roles;
            }
            catch (System.Exception ex)
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
                this.Logger.LogInformation($"Admin get all users attempt by admin: {request.AdminUserId}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.AdminUserId <= 0)
                    throw new ArgumentException("Valid Admin User ID is required");

                if (request.Page < 1)
                    request.Page = 1;

                if (request.Limit < 1 || request.Limit > 100)
                    request.Limit = 10;

                // Validate status if provided
                if (!string.IsNullOrEmpty(request.Status))
                {
                    var validStatuses = new[] { "active", "locked", "inactive" };
                    if (!validStatuses.Contains(request.Status.ToLower()))
                    {
                        throw new ArgumentException($"Invalid status: {request.Status}. Valid statuses are: {string.Join(", ", validStatuses)}");
                    }
                }

                var usersResponse = await this.UserRepository.GetAllUsers(request);

                this.Logger.LogInformation($"Admin get all users successful by admin: {request.AdminUserId}, found {usersResponse.Users.Count} users");

                return usersResponse;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"GetAllUsers Error({ex.Message}) : {ex.InnerException}");
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
                this.Logger.LogInformation($"Admin update user status attempt by admin: {request.AdminUserId} for user: {request.UserId} to status: {request.Status}");

                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (request.AdminUserId <= 0)
                    throw new ArgumentException("Valid Admin User ID is required");

                if (request.UserId <= 0)
                    throw new ArgumentException("Valid User ID is required");

                if (string.IsNullOrEmpty(request.Status))
                    throw new ArgumentException("Status is required");

                // Validate status value
                var validStatuses = new[] { "active", "inactive" };
                if (!validStatuses.Contains(request.Status.ToLower()))
                {
                    throw new ArgumentException($"Invalid status: {request.Status}. Valid statuses are: {string.Join(", ", validStatuses)}");
                }

                var result = await this.UserRepository.UpdateUserStatus(request);

                this.Logger.LogInformation($"Admin update user status successful by admin: {request.AdminUserId} for user: {request.UserId}");

                return result;
            }
            catch (System.Exception ex)
            {
                this.Logger.LogError($"UpdateUserStatus Error({ex.Message}) : {ex.InnerException}");
                throw;
            }
        }
        #endregion
    }
}
