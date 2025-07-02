using TodoApi.Models;

namespace WhiteListing_Backend.Models
{
    public class LoginResponse
    {

        public TokenResponseDto TokenResponse { get; set; }
        public string email { get; set; }
        public string Client_Name { get; set; }

    }
}
