namespace IntelligenceQueryEngine.Models;

public class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // UUID v7 compatible
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public double GenderProbability { get; set; }
    public int Age { get; set; }
    public string AgeGroup { get; set; } = string.Empty;
    public string CountryId { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public double CountryProbability { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}