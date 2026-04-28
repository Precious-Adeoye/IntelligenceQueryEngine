using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services.Contract
{
    public interface IProfileService
    {
        Task<(List<Profile> profiles, int total)> GetProfilesAsync(QueryParams query);
    }
}
