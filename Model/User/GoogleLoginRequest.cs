namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Request model for Google social login
    /// </summary>
    public class GoogleLoginRequest
    {
        /// <summary>
        /// Google ID token (JWT) returned by Google Sign-In
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Tenant ID for multi-tenant support
        /// </summary>
        public int TenantId { get; set; }
    }
}
