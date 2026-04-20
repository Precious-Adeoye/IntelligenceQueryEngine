using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services;

public static class QueryBuilder
{
    public static (string sql, Dictionary<string, object> parameters) Build(QueryParams q)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(q.Gender)) { conditions.Add("Gender = @Gender"); parameters["@Gender"] = q.Gender; }
        if (!string.IsNullOrEmpty(q.AgeGroup)) { conditions.Add("AgeGroup = @AgeGroup"); parameters["@AgeGroup"] = q.AgeGroup; }
        if (!string.IsNullOrEmpty(q.CountryId)) { conditions.Add("CountryId = @CountryId"); parameters["@CountryId"] = q.CountryId; }
        if (q.MinAge.HasValue) { conditions.Add("Age >= @MinAge"); parameters["@MinAge"] = q.MinAge.Value; }
        if (q.MaxAge.HasValue) { conditions.Add("Age <= @MaxAge"); parameters["@MaxAge"] = q.MaxAge.Value; }
        if (q.MinGenderProbability.HasValue) { conditions.Add("GenderProbability >= @MinGP"); parameters["@MinGP"] = q.MinGenderProbability.Value; }
        if (q.MinCountryProbability.HasValue) { conditions.Add("CountryProbability >= @MinCP"); parameters["@MinCP"] = q.MinCountryProbability.Value; }

        var where = conditions.Any() ? $"WHERE {string.Join(" AND ", conditions)}" : "";
        var validSort = new[] { "age", "created_at", "gender_probability" };
        var sortBy = validSort.Contains(q.SortBy?.ToLower()) ? q.SortBy!.ToLower() : "created_at";
        var order = q.Order.ToLower() == "desc" ? "DESC" : "ASC";
        var offset = (q.Page - 1) * q.Limit;

        var sql = $@"
            SELECT * FROM Profiles
            {where}
            ORDER BY {sortBy} {order}
            LIMIT {q.Limit} OFFSET {offset}";

        var countSql = $"SELECT COUNT(*) FROM Profiles {where}";
        return (sql, parameters);
    }
}