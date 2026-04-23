using Microsoft.Data.Sqlite;
using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services;

public class ProfileService
{
    private readonly string _connectionString;

    public ProfileService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=profiles.db";
    }

    public async Task<(List<Profile> profiles, int total)> GetAsync(QueryParams q)
    {
        // FIXED: Now expecting 3 items (sql, parameters, countSql)
        var (sql, parameters, countSql) = QueryBuilder.Build(q);

        var profiles = new List<Profile>();
        int total = 0;

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        // Get total count using the separate countSql
        using var countCmd = new SqliteCommand(countSql, conn);
        foreach (var p in parameters)
            countCmd.Parameters.AddWithValue(p.Key, p.Value);

        total = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        // Get paginated data
        using var cmd = new SqliteCommand(sql, conn);
        foreach (var p in parameters)
            cmd.Parameters.AddWithValue(p.Key, p.Value);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            profiles.Add(new Profile
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                Gender = reader.GetString(2),
                gender_probability = reader.GetDouble(3),
                Age = reader.GetInt32(4),
                age_group = reader.GetString(5),
                country_id = reader.GetString(6),
                CountryName = reader.GetString(7),
                country_probability = reader.GetDouble(8),
                CreatedAt = reader.GetDateTime(9)
            });
        }

        return (profiles, total);
    }
}