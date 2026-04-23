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

        [Column("gender_probability")]
        public double gender_probability { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("age_group")]
        public string age_group { get; set; } = string.Empty;

        [Column("country_id")]
        public string country_id { get; set; } = string.Empty;

        [Column("CountryName")]
        public string CountryName { get; set; } = string.Empty;

        [Column("country_probability")]
        public double country_probability { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}