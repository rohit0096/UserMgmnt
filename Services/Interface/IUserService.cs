using System.Security.Claims;

namespace UserMgmnt.Services.Interface
{
    public interface IUserService
    {
        Task<string> GetUserAsync(ClaimsPrincipal user);
        Task UpdateProfileAsync(string userId, string fileUploadPath, DateTime uploadedDateTime);
    }

}
