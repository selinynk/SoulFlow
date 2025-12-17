using Microsoft.AspNetCore.Identity;

namespace SoulFlow.Models
{
    public class AppUser : IdentityUser
    {
        public string? Interests { get; set; }
        public string? ProfileImage { get; set; }
        public string? Bio { get; set; }
    }
}