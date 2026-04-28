using System;
using Microsoft.AspNetCore.Identity;

namespace IntelligenceQueryEngine.Model.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public long? GitHubId { get; set; }
        public string? GitHubLogin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
