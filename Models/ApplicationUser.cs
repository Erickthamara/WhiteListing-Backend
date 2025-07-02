using Microsoft.AspNetCore.Identity;

namespace WhiteListing_Backend.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string IdNo { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? Client_Id { get; set; }

    }
}
