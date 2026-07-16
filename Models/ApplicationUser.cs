using Microsoft.AspNetCore.Identity;
namespace UtilityManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Company { get; set; }
    }
}