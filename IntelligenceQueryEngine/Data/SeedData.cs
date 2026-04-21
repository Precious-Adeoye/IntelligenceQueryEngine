using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context, IWebHostEnvironment env)
    {
        // Check if already seeded
        if (await context.Profiles.AnyAsync())
        {
            Console.WriteLine("✅ Database already seeded");
            return;
        }

        // Try multiple locations to find the JSON file
        var possiblePaths = new[]
        {
            Path.Combine(env.ContentRootPath, "seed_profiles.json"),
            Path.Combine(env.ContentRootPath, "Data", "seed_profiles.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "seed_profiles.json"),
            Path.Combine(AppContext.BaseDirectory, "seed_profiles.json")
        };

        string? jsonPath = null;
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                jsonPath = path;
                Console.WriteLine($"✅ Found seed file at: {path}");
                break;
            }
        }

        if (jsonPath == null)
        {
            Console.WriteLine("❌ ERROR: seed_profiles.json not found in any location");
            Console.WriteLine("Searched paths:");
            foreach (var path in possiblePaths)
            {
                Console.WriteLine($"  - {path}");
            }
            return;
        }

        Console.WriteLine("📁 Reading seed_profiles.json...");
        var jsonContent = await File.ReadAllTextAsync(jsonPath);

        // Parse the JSON
        using var document = JsonDocument.Parse(jsonContent);
        var root = document.RootElement;

        // Get profiles array (handles both formats)
        JsonElement profilesElement;
        if (root.ValueKind == JsonValueKind.Array)
        {
            profilesElement = root;
        }
        else if (root.TryGetProperty("profiles", out var profilesProp))
        {
            profilesElement = profilesProp;
        }
        else
        {
            Console.WriteLine("❌ ERROR: Invalid JSON format - no profiles array found");
            return;
        }

        var profiles = new List<Profile>();

        foreach (var item in profilesElement.EnumerateArray())
        {
            profiles.Add(new Profile
            {
                Id = Guid.NewGuid().ToString(),
                Name = item.GetProperty("Name").GetString() ?? string.Empty,
                Gender = item.GetProperty("Gender").GetString() ?? string.Empty,
                GenderProbability = item.GetProperty("GenderProbability").GetDouble(),
                Age = item.GetProperty("Age").GetInt32(),
                AgeGroup = item.GetProperty("AgeGroup").GetString() ?? string.Empty,
                CountryId = item.GetProperty("CountryId").GetString() ?? string.Empty,
                CountryName = item.GetProperty("CountryName").GetString() ?? string.Empty,
                CountryProbability = item.GetProperty("CountryProbability").GetDouble(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.Profiles.AddRangeAsync(profiles);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Seeded {profiles.Count} profiles successfully!");
    }
}