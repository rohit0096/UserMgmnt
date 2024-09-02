using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UserMgmnt.Model;
using UserMgmnt.Services.Interface;

namespace UserMgmnt.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public async Task UpdateProfileAsync(string userId, string fileUploadPath, DateTime uploadedDateTime)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.FileUploadPath = fileUploadPath;
            user.UploadedDateTime = uploadedDateTime;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Profile update failed");
            }
        }
    }
}
