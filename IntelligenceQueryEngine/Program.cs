
using IntelligenceQueryEngine.Data;
using IntelligenceQueryEngine.Middleware;
using IntelligenceQueryEngine.Services;
using Microsoft.EntityFrameworkCore;

namespace IntelligenceQueryEngine
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            // Configure for PXXL (port 8080) - MUST be before building app
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            Directory.CreateDirectory("/tmp");

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Services
            builder.Services.AddScoped<ProfileService>();

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

            // CORS
            app.UseCors("AllowAll");
            // Error handling
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseAuthorization();

            app.MapGet("/", () => "Running");
            app.MapControllers();

            // Seed database
             using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                await dbContext.Database.EnsureCreatedAsync();
                await SeedData.InitializeAsync(dbContext, env);
            }

            app.Run();
        }
    }
}
