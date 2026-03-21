using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Tenant.Query.Middleware
{
    /// <summary>
    /// Rate limiting middleware to prevent brute force attacks on authentication endpoints
    /// Limits: 5 login/register attempts per 15 minutes per IP address
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // Track requests per IP: IP -> (count, resetTime)
        private static readonly ConcurrentDictionary<string, (int count, DateTime resetTime)> RequestCounts =
            new ConcurrentDictionary<string, (int, DateTime)>();

        // Configuration
        private const int MaxAttemptsPerWindow = 5;
        private const int WindowDurationMinutes = 15;
        private const int CleanupIntervalMinutes = 30;

        // Cleanup timer to prevent memory leak from accumulated entries
        private static DateTime _lastCleanup = DateTime.UtcNow;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            // Check if this is an auth endpoint that needs rate limiting
            var isAuthEndpoint = request.Path.StartsWithSegments("/api/1.0/user/login") ||
                                 request.Path.StartsWithSegments("/api/1.0/user/login-with-otp") ||
                                 request.Path.StartsWithSegments("/api/1.0/user/register") ||
                                 request.Path.StartsWithSegments("/api/1.0/user/forgot-password");

            if (isAuthEndpoint && !string.IsNullOrEmpty(clientIp))
            {
                // Check rate limit
                if (IsRateLimited(clientIp))
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {ClientIp} on endpoint: {Endpoint}",
                        clientIp, request.Path);

                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many authentication attempts. Please try again later.",
                        retryAfter = WindowDurationMinutes * 60 // Seconds
                    });
                    return;
                }

                // Record this request
                RecordRequest(clientIp);

                // Periodic cleanup to prevent memory leak
                CleanupExpiredEntries();
            }

            await _next(context);
        }

        private bool IsRateLimited(string clientIp)
        {
            if (RequestCounts.TryGetValue(clientIp, out var data))
            {
                var now = DateTime.UtcNow;

                // Check if window has expired
                if (now >= data.resetTime)
                {
                    // Window expired, not rate limited
                    return false;
                }

                // Window still active, check if limit exceeded
                return data.count >= MaxAttemptsPerWindow;
            }

            return false;
        }

        private void RecordRequest(string clientIp)
        {
            var now = DateTime.UtcNow;
            var resetTime = now.AddMinutes(WindowDurationMinutes);

            RequestCounts.AddOrUpdate(clientIp,
                _ => (1, resetTime),
                (_, existing) =>
                {
                    // If window expired, reset counter
                    if (now >= existing.resetTime)
                    {
                        return (1, resetTime);
                    }

                    // Otherwise increment counter
                    return (existing.count + 1, existing.resetTime);
                });
        }

        private void CleanupExpiredEntries()
        {
            var now = DateTime.UtcNow;

            // Only cleanup every N minutes to reduce overhead
            if ((now - _lastCleanup).TotalMinutes < CleanupIntervalMinutes)
            {
                return;
            }

            _lastCleanup = now;

            var expiredKeys = new System.Collections.Generic.List<string>();

            foreach (var kvp in RequestCounts)
            {
                if (now >= kvp.Value.resetTime)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                RequestCounts.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogDebug("Rate limiting cleanup removed {Count} expired entries", expiredKeys.Count);
            }
        }
    }
}
