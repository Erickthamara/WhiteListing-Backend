namespace TodoApi.Models
{
    public class RefreshTokenRequetsDTO
    {
        public Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
