using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UserMgmnt.Model;
using UserMgmnt.Services.Interface;

namespace UserMgmnt.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GetUserAsync(ClaimsPrincipal user)
        {
            try
            {
                var userId = _userManager.GetUserId(user); 

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No user ID found in claims.");
                    throw new Exception("User ID not found in claims");
                }

                var applicationUser = await _userManager.FindByIdAsync(userId);
                if (applicationUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    throw new Exception("User not found");
                }

                _logger.LogInformation("User {UserName} retrieved successfully.", applicationUser.UserName);
                return applicationUser.UserName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user.");
                throw;
            }
        }

        public async Task UpdateProfileAsync(string userId, string fileUploadPath, DateTime uploadedDateTime)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    throw new Exception("User not found");
                }

                user.FileUploadPath = fileUploadPath;
                user.UploadedDateTime = uploadedDateTime;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update profile for user {UserId}. Errors: {Errors}", userId, result.Errors);
                    throw new Exception("Profile update failed");
                }

                _logger.LogInformation("Profile updated successfully for user {UserId}.", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the profile for user {UserId}.", userId);
                throw;
            }
        }
    }
}
