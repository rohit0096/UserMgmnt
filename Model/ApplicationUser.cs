using Microsoft.AspNetCore.Identity;

namespace UserMgmnt.Model
{
    public class ApplicationUser:IdentityUser
    {
        // Additional properties
        public string? FileUploadPath { get; set; } 
        public DateTime UploadedDateTime { get; set; }
    }
}
