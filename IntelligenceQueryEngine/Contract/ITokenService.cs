using System.Security.Claims;

namespace IntelligenceQueryEngine.Contract
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateAccessToken(string token);
        bool ValidateRefreshToken(string refreshToken, out string userId);
    }
}
