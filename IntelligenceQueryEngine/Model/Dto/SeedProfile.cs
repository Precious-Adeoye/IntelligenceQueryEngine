namespace IntelligenceQueryEngine.Model.Dto
{
    public class ProfilesWrapper { public List<SeedProfile> Profiles { get; set; } = new(); }
    public class SeedProfile
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public double GenderProbability { get; set; }
        public int Age { get; set; }
        public string AgeGroup { get; set; } = string.Empty;
        public string CountryId { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public double CountryProbability { get; set; }
    }
}
