using IntelligenceQueryEngine.Data;
using IntelligenceQueryEngine.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntelligenceQueryEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;

        public AdminController(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int limit = 20)
        {
            var users = await _userManager.Users
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.GitHubLogin,
                    u.GitHubId,
                    u.EmailConfirmed
                })
                .ToListAsync();

            var total = await _userManager.Users.CountAsync();

            return Ok(new
            {
                status = "success",
                data = users,
                pagination = new
                {
                    current_page = page,
                    per_page = limit,
                    total_items = total,
                    total_pages = (int)Math.Ceiling((double)total / limit)
                }
            });
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                github_login = user.GitHubLogin,
                github_id = user.GitHubId,
                roles = roles,
                created_at = user.CreatedAt
            });
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "User not found" });

            if (request.Role != "admin" && request.Role != "analyst")
                return BadRequest(new { error = "Role must be 'admin' or 'analyst'" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);

            return Ok(new
            {
                message = $"User role updated to {request.Role}",
                user_id = user.Id,
                new_role = request.Role
            });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetRequestLogs([FromQuery] int page = 1, [FromQuery] int limit = 50)
        {
            var logs = await _dbContext.RequestLogs
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var total = await _dbContext.RequestLogs.CountAsync();

            return Ok(new
            {
                status = "success",
                data = logs,
                pagination = new
                {
                    current_page = page,
                    per_page = limit,
                    total_items = total,
                    total_pages = (int)Math.Ceiling((double)total / limit)
                }
            });
        }

        public class UpdateRoleRequest
        {
            public string Role { get; set; } = string.Empty;
        }
    }
}
