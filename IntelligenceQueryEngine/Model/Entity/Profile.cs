using System.ComponentModel.DataAnnotations.Schema;

namespace IntelligenceQueryEngine.Models
{

    public class Profile
    {
        [Column("Id")]
        public string Id { get; set; } = string.Empty;

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Gender")]
        public string Gender { get; set; } = string.Empty;

        [Column("GenderProbability")]
        public double GenderProbability { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("AgeGroup")]
        public string AgeGroup { get; set; } = string.Empty;

        [Column("CountryId")]
        public string CountryId { get; set; } = string.Empty;

        [Column("CountryName")]
        public string CountryName { get; set; } = string.Empty;

        [Column("CountryProbability")]
        public double CountryProbability { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}