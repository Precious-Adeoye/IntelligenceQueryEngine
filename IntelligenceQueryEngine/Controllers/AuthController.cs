using IntelligenceQueryEngine.Model.Entity;
using IntelligenceQueryEngine.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IntelligenceQueryEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = new HttpClient();
        }

        [HttpGet("github-login")]
        public IActionResult GitHubLogin()
        {
            var redirectUrl = $"{_config["WebPortalUrl"]}/callback.html";
            var clientId = _config["GitHub:ClientId"];
            return Redirect($"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUrl}&scope=user:email");
        }

        [HttpGet("github-callback")]
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest(new { error = "Missing code parameter" });

            // Exchange code for GitHub access token
            var tokenResponse = await _httpClient.PostAsync(
                "https://github.com/login/oauth/access_token",
                new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("client_id", _config["GitHub:ClientId"]!),
                new KeyValuePair<string, string>("client_secret", _config["GitHub:ClientSecret"]!),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", $"{_config["WebPortalUrl"]}/callback.html")
                })
            );

            var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
            var githubToken = System.Web.HttpUtility.ParseQueryString(tokenResult)["access_token"];

            if (string.IsNullOrEmpty(githubToken))
                return Unauthorized(new { error = "Failed to get GitHub token" });

            // Get user info from GitHub
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("InsightaAPI");
            _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", githubToken);

            var userResponse = await _httpClient.GetAsync("https://api.github.com/user");
            var userJson = await userResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(userJson);

            var githubId = doc.RootElement.GetProperty("id").GetInt64();
            var name = doc.RootElement.GetProperty("login").GetString() ?? "unknown";
            var email = doc.RootElement.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;

            // Find or create user
            var user = await _userManager.FindByLoginAsync("GitHub", githubId.ToString());

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email ?? $"{name}@github.user",
                    Email = email,
                    GitHubId = githubId,
                    GitHubLogin = name,
                    EmailConfirmed = email != null
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return BadRequest(createResult.Errors);

                await _userManager.AddLoginAsync(user, new UserLoginInfo("GitHub", githubId.ToString(), "GitHub"));
                await _userManager.AddToRoleAsync(user, "analyst");
            }

            // Sign in
            await _signInManager.SignInAsync(user, isPersistent: true);

            // Generate JWT token
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user.Id, roles.FirstOrDefault() ?? "analyst");

            // Set cookie for web portal
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(15)
            });

            return Redirect($"{_config["WebPortalUrl"]}/dashboard.html");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { error = "No refresh token" });

            // Validate refresh token and get new tokens
            if (!_tokenService.ValidateRefreshToken(refreshToken, out var userId))
                return Unauthorized(new { error = "Invalid refresh token" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(new { error = "User not found" });

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user.Id, roles.FirstOrDefault() ?? "analyst");
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(7)
            });

            return Ok(new { access_token = newAccessToken, expires_in = 900 });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Ok(new { message = "Logged out successfully" });
        }

    }
}
