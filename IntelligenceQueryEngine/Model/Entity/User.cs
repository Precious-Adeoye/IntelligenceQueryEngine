namespace IntelligenceQueryEngine.Model.Entity
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long GithubId { get; set; }
        public string? Email { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = "analyst"; // admin or analyst
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
