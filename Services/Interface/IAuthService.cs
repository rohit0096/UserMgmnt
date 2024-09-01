using Microsoft.AspNetCore.Identity;
using UserMgmnt.Model;

namespace UserMgmnt.Services.Interface
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(Register model);
        Task<string> LoginAsync(Login model);
    }

}
