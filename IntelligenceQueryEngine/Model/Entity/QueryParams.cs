namespace IntelligenceQueryEngine.Models;

public class QueryParams
{
    public string? gender { get; set; }
    public string? age_group { get; set; }
    public string? country_id { get; set; }
    public int? min_age { get; set; }
    public int? max_age { get; set; }
    public double? min_gender_probability { get; set; }
    public double? min_country_probability { get; set; }
    public string? sort_by { get; set; } = "created_at";
    public string order { get; set; } = "asc";
    public int page { get; set; } = 1;
    public int limit { get; set; } = 10;
}