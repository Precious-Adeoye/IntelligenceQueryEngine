using IntelligenceQueryEngine.Model.Entity;

namespace IntelligenceQueryEngine.Contract
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken, User User)?> HandleGitHubCallback(string code);
        Task<(string AccessToken, string RefreshToken)?> RefreshAccessTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task<User?> GetUserByIdAsync(string userId);
        Task<bool> IsAdminAsync(string userId);
    }
}
