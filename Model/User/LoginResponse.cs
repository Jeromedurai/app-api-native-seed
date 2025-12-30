using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user login response
    /// </summary>
    public class LoginResponse
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
        /// Last login date
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Remember me setting
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleInfo> Roles { get; set; } = new List<UserRoleInfo>();

        /// <summary>
        /// Authentication token (if applicable)
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token expiration date (if applicable)
        /// </summary>
        public DateTime? TokenExpiration { get; set; }
    }

    /// <summary>
    /// Model for user role information
    /// </summary>
    public class UserRoleInfo
    {
        /// <summary>
        /// Role ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Role description
        /// </summary>
        public string RoleDescription { get; set; }
    }
}
