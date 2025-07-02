namespace TodoApi.Models
{
    public class TokenResponseDto
    {
        public required string JWTToken { get; set; }
        public required string RefreshToken { get; set; }

    }
}
