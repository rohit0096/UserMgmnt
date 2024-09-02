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

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] IFormFile fileUpload, [FromForm] DateTime uploadedDateTime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Define the directory and file path
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate a unique file name to avoid overwriting
                var fileName = Path.GetFileName(fileUpload.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save the file to the directory
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                // Update profile with file path and datetime
                await _userService.UpdateProfileAsync(userId, filePath, uploadedDateTime);

                return Ok(new { Message = "Profile updated successfully" });
            }

            return BadRequest(new { Message = "Invalid file upload" });
        }
    }
}
