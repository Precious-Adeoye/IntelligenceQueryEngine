using IntelligenceQueryEngine.Data;
using IntelligenceQueryEngine.Model.Entity;

namespace IntelligenceQueryEngine.Middelware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger,
            IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var endpoint = $"{context.Request.Path}{context.Request.QueryString}";
            var method = context.Request.Method;

            // Capture response status code
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

                var statusCode = context.Response.StatusCode;

                // Log asynchronously
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var log = new RequestLog
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = userId,
                            Endpoint = endpoint,
                            Method = method,
                            StatusCode = statusCode,
                            IpAddress = ipAddress,
                            Timestamp = startTime
                        };

                        await dbContext.RequestLogs.AddAsync(log);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save request log");
                    }
                });

                // Log to console
                _logger.LogInformation($"[{startTime:HH:mm:ss}] {method} {endpoint} -> {statusCode} (User: {userId ?? "anonymous"})");
            }
        }
    }
}
