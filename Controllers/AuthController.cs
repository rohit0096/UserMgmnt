using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserMgmnt.Data;
using UserMgmnt.Model;
using UserMgmnt.Services.Interface;

namespace UserMgmnt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            _logger.LogInformation("Registration attempt for user: {Username}", model.Username);

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(model);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Registration failed for user: {Username}", model.Username);

                    // Handle specific error scenarios
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Registration failed for user: {Username}", model.Username);
                        return BadRequest(result.Errors);
                    }
                }

                _logger.LogInformation("Registration success for user: {Username}", model.Username);
                return Ok(new { Result = "Registration successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred during registration: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            _logger.LogInformation("Login attempt for user: {Username}", model.Username);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Login failed for user: {Username}", model.Username);
                    return BadRequest(ModelState);
                }
                var token = await _authService.LoginAsync(model);

                if (token == null)
                {
                    _logger.LogWarning("Login failed for user 401 Permission Denied: {Username}", model.Username);
                    return Unauthorized();
                }
                _logger.LogInformation("Login successful for user: {Username}", model.Username);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred during login: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
