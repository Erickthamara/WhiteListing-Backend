using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApi.Models;
using TodoApi.Services;
using WhiteListing_Backend.Identity;
using WhiteListing_Backend.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhiteListing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly CustomUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Supabase.Client _supabase;
        private readonly IJWTAuthservice _jwtAuthService;
        private readonly ILogger<UserAuthController> _logger;

        public UserAuthController(CustomUserManager userManager,
            SignInManager<ApplicationUser> signInManager, Supabase.Client supabase, IJWTAuthservice jwtAuthService, ILogger<UserAuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _supabase = supabase;
            _jwtAuthService = jwtAuthService;
            _logger = logger;
        }


        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(RegistrationModel request)
        {
            if (request == null)
            {
                return BadRequest("Request is empty");
            }
            var userToBeCreated = new ApplicationUser
            {
                IdNo = request.IdNo,
                Email = request.Email,
                UserName = request.Email

            };
            var result = await _userManager.CreateAsync(userToBeCreated, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });

                var validationProblem = new ValidationProblemDetails(errors);
                return ValidationProblem(validationProblem);

            }

            return Ok("User Successfully Created");
        }


        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginModel request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            // Check incoming cookies
            var incomingCookies = string.Join(", ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
            _logger.LogInformation("Incoming cookies: {Cookies}", incomingCookies);
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);

            _logger.LogInformation("SignIn result: Succeeded={Succeeded}, RequiresTwoFactor={RequiresTwoFactor}, IsLockedOut={IsLockedOut}",
        result.Succeeded, result.RequiresTwoFactor, result.IsLockedOut);

            //==========================================Do not edit this line.=======================================================================================================
            //NB :This is a really important line, it deletes the cookie that is created when the user logs in.
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            //==========================================Do not edit this line.=======================================================================================================



            if (result.Succeeded)
            {

                ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
                TokenResponseDto response = await _jwtAuthService.CreateTokenDuringLoginAsync(user) ?? new TokenResponseDto { JWTToken = null, RefreshToken = null };

                _logger.LogInformation("JWT Token generated: {HasJwt}, RefreshToken generated: {HasRefresh}",
                    !string.IsNullOrEmpty(response.JWTToken),
                    !string.IsNullOrEmpty(response.RefreshToken));

                // Set tokens as HttpOnly cookies
                var jwtCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // set to true in production (HTTPS only)
                    SameSite = SameSiteMode.None,
                    IsEssential = true, // Make sure the cookie is sent even if the user hasn't consented to non-essential cookies
                    //Domain = ".erickthamara.com",
                    Expires = DateTime.UtcNow.AddMinutes(15)
                };

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    //Domain = ".erickthamara.com",
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("jwt_token", response.JWTToken ?? "", jwtCookieOptions);
                Response.Cookies.Append("refresh_token", response.RefreshToken ?? "", refreshCookieOptions);

                _logger.LogInformation("Cookies set: jwt_token, refresh_token");

                // Return only user data (not tokens)
                var loginResponse = new
                {
                    email = user.Email,
                    client_name = user.Email // Replace with actual client name if different
                };

                return Ok(loginResponse);
            }
            if (result.RequiresTwoFactor)
            {
                // return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                //_logger.LogWarning(2, "User account locked out.");
                //return View("Lockout");
                return Forbid("User account locked out.");
            }
            else
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                return BadRequest("Inavlid Email or Password");
            }
        }

        //[Authorize]
        //[HttpPost("refresh")]
        //public async Task<ActionResult<TokenResponseDto>> Refresh(RefreshTokenRequetsDTO request)
        //{
        //    var response = await _jwtAuthService.RefreshTokensAsync(request);
        //    if (response == null)
        //        return BadRequest("Invalid Token");
        //    return Ok(response);
        //}

        [AllowAnonymous] // Remove [Authorize] so even expired sessions can call this
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequetsDTO request)
        {

            _logger.LogInformation("Refresh endpoint hit");
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token found");

            // Validate the refresh token and get new tokens
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh");

            var response = await _jwtAuthService.RefreshTokensAsync(refreshToken);
            if (response == null)
                return Unauthorized("Invalid or expired refresh token");

            // Write new tokens to cookies
            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                //Domain = ".erickthamara.com",
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                //Domain = ".erickthamara.com",
                Expires = DateTime.UtcNow.AddDays(7)
            };
            _logger.LogInformation("Ammending the tokens");
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Append("jwt_token", response.JWTToken!, accessOptions);
            Response.Cookies.Delete("refresh_token");
            Response.Cookies.Append("refresh_token", response.RefreshToken!, refreshOptions);

            _logger.LogInformation("Tokens Refreshed");

            return Ok(new { message = "Tokens refreshed" });
        }



        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1) // Expire immediately
            };

            // Clear both JWT and Refresh Token cookies
            Response.Cookies.Append("jwt_token", "", cookieOptions);
            Response.Cookies.Append("refresh_token", "", cookieOptions);

            return Ok(new { message = "Logged out successfully" });
        }


        [Authorize]
        [HttpGet("AuthTest")]
        public ActionResult<string> AuthTest()
        {
            return Ok("You are authenticated");
        }


        [Authorize]
        [HttpGet("CheckAuth")]
        public ActionResult<string> CheckIfSignedIn()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { signedIn = false });
            }

            return Ok(new { signedIn = true, userId });
        }

        [HttpGet("ApiHelloWorld")]
        public string ApiHelloWorld()
        {
            return "Hello world,i am now live.";
        }


        protected virtual async Task<Microsoft.AspNetCore.Identity.SignInResult> PasswordSignInAsyncUsingIDNO(string idNo, string password,
        bool isPersistent, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByID_NOAsync(idNo);
            //var user = await _userManager.FindByNameAsync(idNo);
            if (user == null)
            {
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }
}
