using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserMgmnt.Services.Interface;

namespace UserMgmnt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var userName = _userService.GetUserId(User);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }
            // Fetch user profile data by userId (can be added to the service)
            return Ok(new { userName = userName });
        }
    }
}
