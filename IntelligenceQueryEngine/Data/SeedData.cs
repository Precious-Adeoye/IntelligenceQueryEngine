using IntelligenceQueryEngine.Model.Dto;
using IntelligenceQueryEngine.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IntelligenceQueryEngine.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context, IWebHostEnvironment env)
        {
            if (await context.Profiles.AnyAsync()) return;

            var jsonPath = Path.Combine(env.ContentRootPath, "seed_profiles.json");
            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var wrapper = JsonSerializer.Deserialize<ProfilesWrapper>(jsonContent);

            if (wrapper?.Profiles == null) return;

            var profiles = wrapper.Profiles.Select(p => new Profile
            {
                Name = p.Name,
                Gender = p.Gender,
                GenderProbability = p.GenderProbability,
                Age = p.Age,
                AgeGroup = p.AgeGroup,
                CountryId = p.CountryId,
                CountryName = p.CountryName,
                CountryProbability = p.CountryProbability,
                CreatedAt = DateTime.UtcNow
            });

            await context.Profiles.AddRangeAsync(profiles);
            await context.SaveChangesAsync();
        }

    }

}
