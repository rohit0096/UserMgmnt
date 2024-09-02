using System.Security.Claims;

namespace UserMgmnt.Services.Interface
{
    public interface IUserService
    {
        string GetUserId(ClaimsPrincipal user);
        Task UpdateProfileAsync(string userId, string fileUploadPath, DateTime uploadedDateTime);
    }

}
