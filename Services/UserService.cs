using System.Security.Claims;
using UserMgmnt.Services.Interface;

namespace UserMgmnt.Services
{
    public class UserService : IUserService
    {
        public string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }
    }
}
