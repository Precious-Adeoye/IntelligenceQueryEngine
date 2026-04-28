using IntelligenceQueryEngine.Data;
using IntelligenceQueryEngine.Model.Entity;
using IntelligenceQueryEngine.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntelligenceQueryEngine.Services.Implementation
{
    public class DatabaseSeedService : IDatabaseSeedService
    {
        private readonly AppDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DatabaseSeedService(
            AppDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
        }
        public async Task SeedAllAsync()
        {
            await _dbContext.Database.EnsureCreatedAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedProfilesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "admin", "analyst" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

        }

        private async Task SeedAdminUserAsync()
        {
            if (await _userManager.Users.AnyAsync(u => u.Email == "admin@insighta.com"))
                return;

            var admin = new ApplicationUser
            {
                UserName = "admin@insighta.com",
                Email = "admin@insighta.com",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(admin, "Admin@123");
            await _userManager.AddToRoleAsync(admin, "admin");
        }

        private async Task SeedProfilesAsync()
        {
            await SeedData.InitializeAsync(_dbContext, _env);
        }
    }
}
