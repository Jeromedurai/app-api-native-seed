using System;

namespace Tenant.Query.Model.Authentication
{
    /// <summary>
    /// Token response model
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token expiration timestamp (Unix timestamp)
        /// </summary>
        public long Exp { get; set; }

        /// <summary>
        /// Token expiration date time (ISO 8601 format)
        /// </summary>
        public string ExpiresAt { get; set; }
    }
}

