namespace IntelligenceQueryEngine.Model.Dto
{
    public class ProfilesWrapper { public List<SeedProfile> profiles { get; set; } = new(); }
    public class SeedProfile
    {
        public string name { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public double gender_probability { get; set; }
        public int age { get; set; }
        public string age_group { get; set; } = string.Empty;
        public string country_id { get; set; } = string.Empty;
        public string country_name { get; set; } = string.Empty;
        public double country_probability { get; set; }
    }
}
