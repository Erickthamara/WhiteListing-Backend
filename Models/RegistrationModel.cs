using System.ComponentModel.DataAnnotations;

namespace WhiteListing_Backend.Models
{
    public class RegistrationModel
    {
        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Must be between 6 and 8 characters long.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "ID number must contain digits only.")]
        required public string IdNo { get; set; }


        //[Required]
        [EmailAddress]
        required public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?""{}|<>]).+$", ErrorMessage = "Password must contain at least one special character.")]
        required public string Password { get; set; }
    }
}
