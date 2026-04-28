using IntelligenceQueryEngine.Middelware;
using IntelligenceQueryEngine.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace IntelligenceQueryEngine.Extentions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowAll");
            return app;
        }

        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }


        public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
        {
            // MapGet is an extension on IEndpointRouteBuilder, so use the endpoint routing helper.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Running");
                });

                endpoints.MapGet("/health", async context =>
                {
                    var payload = new { status = "healthy", timestamp = DateTime.UtcNow };
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                });
            });

            return app;
        }

        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseMiddleware<RateLimitMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
            return app;
        }
    }
}
