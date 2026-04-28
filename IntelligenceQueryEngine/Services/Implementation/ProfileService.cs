using Microsoft.Data.Sqlite;
using IntelligenceQueryEngine.Models;
using IntelligenceQueryEngine.Services.Contract;

namespace IntelligenceQueryEngine.Services.Implementation;

public class ProfileService : IProfileService
{
    private readonly string _connectionString;

    public ProfileService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=profiles.db";
    }

    public async Task<(List<Profile> profiles, int total)> GetProfilesAsync(QueryParams query)
    {
        if (query == null)
            query = new QueryParams();

        // Build SQL query and parameters
        var (sql, parameters, countSql) = QueryBuilder.Build(query);

        var profiles = new List<Profile>();
        int total = 0;

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        // ==========================================
        // Execute COUNT query
        // ==========================================
        using var countCmd = new SqliteCommand(countSql, conn);
        foreach (var param in parameters)
        {
            countCmd.Parameters.AddWithValue(param.Key, param.Value);
        }
        total = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        // ==========================================
        // Execute DATA query
        // ==========================================
        using var cmd = new SqliteCommand(sql, conn);
        foreach (var param in parameters)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            profiles.Add(new Profile
            {
                id = reader.GetString(0),
                name = reader.GetString(1),
                gender = reader.GetString(2),
                gender_probability = reader.GetDouble(3),
                age = reader.GetInt32(4),
                age_group = reader.GetString(5),
                country_id = reader.GetString(6),
                country_name = reader.GetString(7),
                country_probability = reader.GetDouble(8),
                created_at = reader.GetDateTime(9)
            });
        }

        return (profiles, total);
    }
}