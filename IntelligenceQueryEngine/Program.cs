using IntelligenceQueryEngine.Data;
using IntelligenceQueryEngine.Extentions;
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
           
            // Bind to hosting port
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddIdentityServices();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCustomServices();
            builder.Services.AddCorsPolicy();
            builder.Services.AddSwaggerDocs();
            builder.Services.AddControllersWithJson();
            builder.Services.AddHttpClient();



            var app = builder.Build();

            app.UseSwaggerDocs();
            app.UseCorsPolicy();
            app.UseGlobalErrorHandler();
            app.UseSecurity();
            app.UseHealthCheck();
            app.MapControllers();
            
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