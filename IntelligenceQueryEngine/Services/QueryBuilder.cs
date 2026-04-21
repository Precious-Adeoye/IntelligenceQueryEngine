using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services;

public static class QueryBuilder
{
    public static (string sql, Dictionary<string, object> parameters, string countSql) Build(QueryParams q)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(q.Gender))
        {
            conditions.Add("Gender = @Gender");
            parameters["@Gender"] = q.Gender;
        }

        if (!string.IsNullOrEmpty(q.AgeGroup))
        {
            conditions.Add("AgeGroup = @AgeGroup");
            parameters["@AgeGroup"] = q.AgeGroup;
        }

        if (!string.IsNullOrEmpty(q.CountryId))
        {
            conditions.Add("CountryId = @CountryId");
            parameters["@CountryId"] = q.CountryId;
        }

        if (q.MinAge.HasValue)
        {
            conditions.Add("Age >= @MinAge");
            parameters["@MinAge"] = q.MinAge.Value;
        }

        if (q.MaxAge.HasValue)
        {
            conditions.Add("Age <= @MaxAge");
            parameters["@MaxAge"] = q.MaxAge.Value;
        }

        if (q.MinGenderProbability.HasValue)
        {
            conditions.Add("GenderProbability >= @MinGP");
            parameters["@MinGP"] = q.MinGenderProbability.Value;
        }

        if (q.MinCountryProbability.HasValue)
        {
            conditions.Add("CountryProbability >= @MinCP");
            parameters["@MinCP"] = q.MinCountryProbability.Value;
        }

        var whereClause = conditions.Any()
            ? $"WHERE {string.Join(" AND ", conditions)}"
            : string.Empty;

        // Valid sort columns
        var validSortColumns = new[] { "age", "created_at", "gender_probability" };
        var sortBy = validSortColumns.Contains(q.SortBy?.ToLower())
            ? q.SortBy!.ToLower()
            : "created_at";
        var order = q.Order.ToLower() == "desc" ? "DESC" : "ASC";

        // Pagination
        var offset = (q.Page - 1) * q.Limit;
        var limit = Math.Min(q.Limit, 50);

        // Data query
        var sql = $@"
            SELECT * FROM Profiles
            {whereClause}
            ORDER BY {sortBy} {order}
            LIMIT {limit} OFFSET {offset}";

        // Count query (separate, no LIMIT/OFFSET)
        var countSql = $@"
            SELECT COUNT(*) FROM Profiles
            {whereClause}";

        // Return 3 items: sql, parameters, countSql
        return (sql, parameters, countSql);
    }
}