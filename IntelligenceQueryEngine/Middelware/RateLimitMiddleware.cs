using System.Collections.Concurrent;

namespace IntelligenceQueryEngine.Middelware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, UserRateLimit> _userLimits = new();
        private static readonly ConcurrentDictionary<string, UserRateLimit> _ipLimits = new();

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            int limit = 100; // Analyst default
            var role = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role == "admin")
                limit = 200;
            else if (role == "analyst")
                limit = 100;
            else
                limit = 50; // Unauthenticated

            // Check rate limit by user ID (if authenticated) or by IP
            bool isAllowed;
            if (!string.IsNullOrEmpty(userId))
                isAllowed = IsRequestAllowed(_userLimits, userId, limit);
            else
                isAllowed = IsRequestAllowed(_ipLimits, ipAddress, limit);

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"status\":\"error\",\"message\":\"Rate limit exceeded. Please try again later.\"}");
                return;
            }

            await _next(context);
        }

        private bool IsRequestAllowed(ConcurrentDictionary<string, UserRateLimit> dictionary, string key, int limit)
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-1);

            var record = dictionary.GetOrAdd(key, new UserRateLimit());

            lock (record.Lock)
            {
                // Clean up old requests
                record.RequestTimes.RemoveAll(t => t < windowStart);

                if (record.RequestTimes.Count >= limit)
                    return false;

                record.RequestTimes.Add(now);
                return true;
            }
        }

        private class UserRateLimit
        {
            public List<DateTime> RequestTimes { get; set; } = new();
            public object Lock { get; set; } = new();
        }

    }
}
