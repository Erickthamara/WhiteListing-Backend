using System.ComponentModel.DataAnnotations;

namespace WhiteListing_Backend.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessage = "ID number is required.")]
        //[StringLength(8, MinimumLength = 6, ErrorMessage = "Must be between 6 and 8 characters long.")]
        //[RegularExpression(@"^\d+$", ErrorMessage = "ID number must contain digits only.")]
        //public string IdNo { get; set; }


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?""{}|<>]).+$", ErrorMessage = "Password must contain at least one special character.")]
        public string Password { get; set; }
    }

}
