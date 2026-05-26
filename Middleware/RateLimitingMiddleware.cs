using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Tenant.Query.Middleware
{
    /// <summary>
    /// Rate limiting middleware to prevent brute force attacks on authentication endpoints.
    /// Password auth and OTP auth use separate per-IP counters (5 attempts per 15 minutes each).
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // Track requests per IP + bucket: key -> (count, resetTime)
        private static readonly ConcurrentDictionary<string, (int count, DateTime resetTime)> RequestCounts =
            new ConcurrentDictionary<string, (int, DateTime)>();

        // Configuration
        private const int MaxAttemptsPerWindow = 5;
        private const int WindowDurationMinutes = 15;
        private const int CleanupIntervalMinutes = 30;

        private const string PasswordAuthBucket = "password";
        private const string OtpAuthBucket = "otp";

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
            var rateLimitBucket = GetRateLimitBucket(request.Path);

            if (rateLimitBucket != null && !string.IsNullOrEmpty(clientIp))
            {
                var bucketKey = $"{clientIp}:{rateLimitBucket}";

                if (IsRateLimited(bucketKey))
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {ClientIp} on endpoint: {Endpoint} (bucket: {Bucket})",
                        clientIp, request.Path, rateLimitBucket);

                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many authentication attempts. Please try again later.",
                        retryAfter = WindowDurationMinutes * 60 // Seconds
                    });
                    return;
                }

                RecordRequest(bucketKey);
                CleanupExpiredEntries();
            }

            await _next(context);
        }

        private static string GetRateLimitBucket(PathString path)
        {
            if (path.StartsWithSegments("/api/user/auth/request-login-otp") ||
                path.StartsWithSegments("/api/user/auth/login-with-otp") ||
                path.StartsWithSegments("/api/user/auth/resend-login-otp") ||
                path.StartsWithSegments("/api/1.0/user/login-with-otp"))
            {
                return OtpAuthBucket;
            }

            if (path.StartsWithSegments("/api/user/auth/login") ||
                path.StartsWithSegments("/api/1.0/user/login") ||
                path.StartsWithSegments("/api/1.0/user/register") ||
                path.StartsWithSegments("/api/1.0/user/forgot-password") ||
                path.StartsWithSegments("/api/user/auth/forgot-password"))
            {
                return PasswordAuthBucket;
            }

            return null;
        }

        private bool IsRateLimited(string bucketKey)
        {
            if (RequestCounts.TryGetValue(bucketKey, out var data))
            {
                var now = DateTime.UtcNow;

                if (now >= data.resetTime)
                {
                    return false;
                }

                return data.count >= MaxAttemptsPerWindow;
            }

            return false;
        }

        private void RecordRequest(string bucketKey)
        {
            var now = DateTime.UtcNow;
            var resetTime = now.AddMinutes(WindowDurationMinutes);

            RequestCounts.AddOrUpdate(bucketKey,
                _ => (1, resetTime),
                (_, existing) =>
                {
                    if (now >= existing.resetTime)
                    {
                        return (1, resetTime);
                    }

                    return (existing.count + 1, existing.resetTime);
                });
        }

        private void CleanupExpiredEntries()
        {
            var now = DateTime.UtcNow;

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
