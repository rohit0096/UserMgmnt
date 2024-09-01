using System.Security.Claims;

namespace UserMgmnt.Services.Interface
{
    public interface IUserService
    {
        string GetUserId(ClaimsPrincipal user);
        // Add more methods for user-related operations if needed
    }

}
