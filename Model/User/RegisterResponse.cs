using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user registration response
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// User information
        /// </summary>
        public RegisteredUser User { get; set; }

        /// <summary>
        /// JWT authentication token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh token for token renewal
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Token expiration time in seconds
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Token expiration timestamp
        /// </summary>
        public DateTime TokenExpiration { get; set; }
    }

    /// <summary>
    /// Model for registered user information
    /// </summary>
    public class RegisteredUser
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Whether email is verified
        /// </summary>
        public bool EmailVerified { get; set; }

        /// <summary>
        /// Whether phone is verified
        /// </summary>
        public bool PhoneVerified { get; set; }

        /// <summary>
        /// Account creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleInfo> Roles { get; set; } = new List<UserRoleInfo>();
    }
}
