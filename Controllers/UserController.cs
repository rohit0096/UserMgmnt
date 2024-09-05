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
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var userName = _userService.GetUserId(User);
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("You are Unauthorized  user: {Username}", userName);
                    return Unauthorized();
                }
                // Fetch user profile data by userId (can be added to the service)  
                _logger.LogInformation("user profile Retrival : {Username}", userName);
                return Ok(new { userName = userName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile: {Username}", userName);
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] IFormFile fileUpload, [FromForm] DateTime uploadedDateTime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                _logger.LogInformation("User {UserId} is attempting to update their profile.", userId);

                if (fileUpload != null && fileUpload.Length > 0)
                {
                    // Define the directory and file path  
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                    _logger.LogInformation("Upload directory set to {UploadsDir}.", uploadsDir);

                    // Check if the directory exists, if not, create it  
                    if (!Directory.Exists(uploadsDir))
                    {
                        _logger.LogInformation("Directory {UploadsDir} does not exist. Creating directory.", uploadsDir);
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Generate a unique file name to avoid overwriting  
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);
                    _logger.LogInformation("Saving uploaded file to {FilePath}.", filePath);

                    // Save the file to the directory  
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(stream);
                    }

                    // Update profile with file path and datetime  
                    await _userService.UpdateProfileAsync(userId, filePath, uploadedDateTime);
                    _logger.LogInformation("User {UserId} successfully updated their profile with file {FileName}.", userId, fileName);

                    return Ok(new { Message = "Profile updated successfully" });
                }
                _logger.LogWarning("User {UserId} attempted to upload an invalid file.", userId);
                return BadRequest(new { Message = "Invalid file upload" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {UserId}", userId);
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }
    }
}
