using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserMgmnt.Model;
using UserMgmnt.Services.Interface;

namespace UserMgmnt.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager,
                           IOptions<JwtSettings> jwtSettings,
                           ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(Register model)
        {
            _logger.LogInformation("Attempting to register user: {Username}", model.Username);
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser!=null)
            {
                _logger.LogWarning("Registration failed: Username {Username} is already taken.", model.Username);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameTaken",
                    Description = $"Username '{model.Username}' is already taken."
                });
            }

            _logger.LogInformation("Attempting to register user: {Username}", model.Username);
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Username} registered successfully.", model.Username);
            }
            else
            {
                _logger.LogWarning("User {Username} registration failed. Errors: {Errors}",
                    model.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }

        public async Task<string> LoginAsync(Login model)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", model.Username);
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User {Username} not found.", model.Username);
                    return null;
                }
                _logger.LogInformation("Login attempt for password: {Password}", model.Password);
                var validpassword = await _userManager.CheckPasswordAsync(user,model.Password);

                if (!validpassword)
                {
                    _logger.LogWarning("Login failed: User {Password} not found.", model.Password);
                    return null;
                }

                _logger.LogInformation("Login successful for user: {Username}", model.Username);
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user {Username}.", model.Username);
                throw;
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation("Generating JWT token for user: {Username}", user.UserName);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                _logger.LogInformation("JWT token generated successfully for user: {Username}", user.UserName);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating JWT token for user: {Username}.", user.UserName);
                throw;
            }
        }
    }

}
