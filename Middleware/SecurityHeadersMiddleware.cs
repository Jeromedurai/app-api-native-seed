using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Tenant.Query.Middleware
{
    /// <summary>
    /// Middleware to add security headers to all HTTP responses
    /// Helps prevent common web vulnerabilities:
    /// - X-Content-Type-Options: Prevents MIME type sniffing attacks
    /// - X-Frame-Options: Prevents clickjacking by restricting where the page can be embedded
    /// - Content-Security-Policy: Controls which resources can be loaded
    /// - Strict-Transport-Security: Forces HTTPS connections
    /// - X-XSS-Protection: Enables XSS protection in browsers
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers to response
            var headers = context.Response.Headers;

            // ✅ Prevent MIME type sniffing (e.g., treating .txt as .js)
            headers["X-Content-Type-Options"] = "nosniff";

            // ✅ Prevent clickjacking by disallowing embedding in frames
            headers["X-Frame-Options"] = "DENY";

            // ✅ Enable XSS protection in older browsers
            headers["X-XSS-Protection"] = "1; mode=block";

            // ✅ Enforce HTTPS connections for 1 year (31536000 seconds)
            // Includes subdomains; preload allows inclusion in browser preload lists
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";

            // ✅ Referrer Policy - Control how much referrer info is sent
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // ✅ Content Security Policy (CSP)
            // Restrict script execution to same origin and trusted CDNs
            headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' https://checkout.razorpay.com https://cdn.jsdelivr.net; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
                "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net; " +
                "connect-src 'self' https://checkout.razorpay.com https://api.razorpay.com; " +
                "img-src 'self' data: https:; " +
                "frame-src 'self' https://checkout.razorpay.com; " +
                "object-src 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self'; " +
                "upgrade-insecure-requests";

            // ✅ Permissions Policy (formerly Feature Policy)
            // Control which browser features can be used
            headers["Permissions-Policy"] =
                "geolocation=(), " +
                "microphone=(), " +
                "camera=(), " +
                "payment=(self \"https://checkout.razorpay.com\"), " +
                "usb=()";

            await _next(context);
        }
    }
}
