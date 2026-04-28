using IntelligenceQueryEngine.Data;
using IntelligenceQueryEngine.Middleware;
using IntelligenceQueryEngine.Services.Implementation;
using Microsoft.EntityFrameworkCore;

namespace IntelligenceQueryEngine
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });

            // Swagger / OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // HttpClient
            builder.Services.AddHttpClient();

            // Bind to hosting port
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            // Writable temp folder
            //Directory.CreateDirectory("/tmp");

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=app.db"));

            // Health checks
           //builder.Services.AddHealthChecks();

            // App services
            builder.Services.AddScoped<ProfileService>();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Middleware
            app.UseCors("AllowAll");
           // app.UseMiddleware<ErrorHandlingMiddleware>();

            // Enable Swagger in all environments (temporary for debugging)
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            // Health route
            app.MapGet("/", () => "Running");

           // app.MapHealthChecks("/health");

            app.MapControllers();

            // Create database only (non-blocking seed removed for deployment stability)
            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                // THIS IS CRITICAL - Creates the database and tables
                await dbContext.Database.EnsureCreatedAsync();

                // THEN seed the data
                await SeedData.InitializeAsync(dbContext, env);
            }

            try
            {
                Console.WriteLine("APP STARTING...");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application failed to start: " + ex);
                throw;
            }
        }
    }
}