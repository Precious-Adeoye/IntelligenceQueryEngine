using System.ComponentModel.DataAnnotations.Schema;

namespace IntelligenceQueryEngine.Models
{

    public class Profile
    {
        [Column("id")]
        public string id { get; set; } = string.Empty;

        [Column("name")]
        public string name { get; set; } = string.Empty;

        [Column("gender")]
        public string gender { get; set; } = string.Empty;
        [Column("gender_probability")]
        public double gender_probability { get; set; }

        [Column("age")]
        public int age { get; set; }

        [Column("age_group")]
        public string age_group { get; set; } = string.Empty;

        [Column("country_id")]
        public string country_id { get; set; } = string.Empty;

        [Column("country_name")]
        public string country_name { get; set; } = string.Empty;

        [Column("country_probability")]
        public double country_probability { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; }
    }
}