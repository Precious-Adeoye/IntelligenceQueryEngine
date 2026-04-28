using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services.Implementation;

public static class QueryBuilder
{
    public static (string sql, Dictionary<string, object> parameters, string countSql) Build(QueryParams q)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        // Gender filter
        if (!string.IsNullOrEmpty(q.gender))
        {
            conditions.Add("gender = @gender");
            parameters["@gender"] = q.gender;
        }

        // Age group filter
        if (!string.IsNullOrEmpty(q.age_group))
        {
            conditions.Add("age_group = @age_group");
            parameters["@age_group"] = q.age_group;
        }

        // Country filter
        if (!string.IsNullOrEmpty(q.country_id))
        {
            conditions.Add("country_id = @country_id");
            parameters["@country_id"] = q.country_id;
        }

        // Min age
        if (q.min_age.HasValue)
        {
            conditions.Add("age >= @min_age");
            parameters["@min_age"] = q.min_age.Value;
        }

        // Max age
        if (q.max_age.HasValue)
        {
            conditions.Add("age <= @max_age");
            parameters["@max_age"] = q.max_age.Value;
        }

        // Min gender probability
        if (q.min_gender_probability.HasValue)
        {
            conditions.Add("gender_probability >= @min_gender_probability");
            parameters["@min_gender_probability"] = q.min_gender_probability.Value;
        }

        // Min country probability
        if (q.min_country_probability.HasValue)
        {
            conditions.Add("country_probability >= @min_country_probability");
            parameters["@min_country_probability"] = q.min_country_probability.Value;
        }

        // Build WHERE clause
        var whereClause = conditions.Any()
            ? $"WHERE {string.Join(" AND ", conditions)}"
            : string.Empty;

        // Sorting
        var validSortColumns = new[] { "age", "created_at", "gender_probability" };
        var sortBy = validSortColumns.Contains(q.sort_by?.ToLower())
            ? q.sort_by!.ToLower()
            : "created_at";
        var order = q.order?.ToLower() == "desc" ? "DESC" : "ASC";

        // Pagination
        var offset = (q.page - 1) * q.limit;
        var limit = Math.Min(q.limit, 50);

        // DATA QUERY - explicit column order matching Profile properties
        var sql = $@"
            SELECT id, name, gender, gender_probability, age, age_group, country_id, country_name, country_probability, created_at
            FROM profiles
            {whereClause}
            ORDER BY {sortBy} {order}
            LIMIT {limit} OFFSET {offset}";

        // COUNT QUERY
        var countSql = $@"
            SELECT COUNT(*) FROM profiles
            {whereClause}";

        return (sql, parameters, countSql);
    }
}