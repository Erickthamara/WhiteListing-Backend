using TodoApi.Models;
using WhiteListing_Backend.Models;

namespace TodoApi.Services
{
    public interface IJWTAuthservice
    {
        //Task<User?> RegisterAsync(User request);
        Task<TokenResponseDto?> CreateTokenDuringLoginAsync(ApplicationUser request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequetsDTO request);
    }
}
