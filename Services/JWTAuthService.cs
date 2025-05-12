using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoApi.Models;
using WhiteListing_Backend.Models;

namespace TodoApi.Services
{

    public class JWTAuthService : IJWTAuthservice
    {

        private readonly Supabase.Client _supabase;
        private readonly IConfiguration _configuration;

        public JWTAuthService(IConfiguration configuration, Supabase.Client supabase)
        {
            _supabase = supabase;
            _configuration = configuration;
        }

        //========================================LOGIN USER========================
        public async Task<TokenResponseDto?> CreateTokenDuringLoginAsync(ApplicationUser request)
        {

            var user = await _supabase.From<SupabaseUserModel>().Where(M => M.Id == request.Id).Get();
            if (user is null)
            {
                return null;
            }
            //var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, request.HashedPassword);
            //if (result != PasswordVerificationResult.Success)
            //{
            //    return null;
            //}

            return await CreateTokens(request);
        }





        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequetsDTO request)
        {
            try
            {
                var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

                if (user == null)
                {
                    return null;  // Invalid token, no user found
                }

                // Proceed to create a new token if the user is found
                ApplicationUser? userModel = new ApplicationUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IdNo = user.IdNo,
                };

                return await CreateTokens(userModel);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                //_logger.LogError($"Error in RefreshTokensAsync: {ex.Message}");

                return null;  // Optionally, handle errors here as needed
            }
        }

        private async Task<SupabaseUserModel?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _supabase.From<SupabaseUserModel>().Where(M => M.Id == userId).Get();
            var userModel = user.Models.FirstOrDefault();


            if (userModel is null || userModel?.RefreshToken != refreshToken || userModel.RefreshTokenExpiryTime < DateTime.Now)
            {
                return null;
            }
            return userModel;
        }
        private async Task<TokenResponseDto?> CreateTokens(ApplicationUser user)
        {
            return new TokenResponseDto()
            {
                JWTToken = CreateToken(user),
                RefreshToken = await SaveAndGeneratedRefreshToken(user)
            };
        }

        private async Task<string> SaveAndGeneratedRefreshToken(ApplicationUser user)
        {
            var refreshToken = GenerateRefreshToken();
            //user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            var update = await _supabase
                  .From<SupabaseUserModel>()
                  .Where(x => x.Id == user.Id)
                  .Set(x => x.RefreshToken, refreshToken)
                  .Set(x => x.RefreshTokenExpiryTime, DateTime.Now.AddDays(7))
                  .Update();
            return refreshToken;

        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string CreateToken(ApplicationUser user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                //new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);


            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }


        //private Boolean CheckIfTokenIsExpired()
        //{

        //    return false;
        //}


    }
}
