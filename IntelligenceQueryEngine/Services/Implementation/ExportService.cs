using IntelligenceQueryEngine.Models;
using IntelligenceQueryEngine.Services.Contract;
using Microsoft.Data.Sqlite;
using System.Text;

namespace IntelligenceQueryEngine.Services.Implementation
{
    public class ExportService : IExportService
    {
        private readonly string _connectionString;

        public ExportService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
        }

        public async Task<byte[]> ExportProfilesToCsvAsync(QueryParams query)
        {
            var (sql, parameters, _) = QueryBuilder.Build(query);
            var profiles = new List<Profile>();

            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqliteCommand(sql, conn);
            foreach (var param in parameters)
                cmd.Parameters.AddWithValue(param.Key, param.Value);

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

            return GenerateCsv(profiles);
        }

        private byte[] GenerateCsv(List<Profile> profiles)
        {
            var sb = new StringBuilder();

            // Headers
            sb.AppendLine("id,name,gender,gender_probability,age,age_group,country_id,country_name,country_probability,created_at");

            // Rows
            foreach (var p in profiles)
            {
                sb.AppendLine($"\"{p.id}\",\"{p.name}\",\"{p.gender}\",{p.gender_probability},{p.age},\"{p.age_group}\",\"{p.country_id}\",\"{p.country_name}\",{p.country_probability},\"{p.created_at:O}\"");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public string GetCsvFileName(QueryParams query)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            return $"insighta_export_{timestamp}.csv";
        }
    }
}
