using System.Security.Claims;

namespace IntelligenceQueryEngine.Contract
{
    public interface IRoleService
    {
        bool IsAdmin(ClaimsPrincipal user);
        bool IsAnalyst(ClaimsPrincipal user);
        string GetUserRole(ClaimsPrincipal user);
        Task<bool> HasPermissionAsync(string userId, string requiredRole);
    }
}
