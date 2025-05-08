using Microsoft.AspNetCore.Identity;

namespace WhiteListing_Backend.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string IdNo { get; set; }

    }
}
