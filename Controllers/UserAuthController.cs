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
        //private readonly ILogger _logger;

        public UserAuthController(CustomUserManager userManager,
            SignInManager<ApplicationUser> signInManager, Supabase.Client supabase, IJWTAuthservice jwtAuthService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _supabase = supabase;
            _jwtAuthService = jwtAuthService;
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
        public async Task<ActionResult<TokenResponseDto>> Login(LoginModel request)
        {

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);


            //==========================================Do not edit this line.=======================================================================================================
            //NB :This is a really important line, it deletes the cookie that is created when the user logs in.
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            //==========================================Do not edit this line.=======================================================================================================


            if (result == null)
            {
                return BadRequest("Invalid Username or Password.");
            }
            if (result.Succeeded)
            {
                //_logger.LogInformation(1, "User logged in.");
                TokenResponseDto response = await _jwtAuthService.CreateTokenDuringLoginAsync(await _userManager.FindByEmailAsync(request.Email));

                return Ok(response);
            }
            if (result.RequiresTwoFactor)
            {
                //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                //_logger.LogWarning(2, "User account locked out.");
                //return View("Lockout");
                return Forbid("User account locked out.");
            }
            else
            {
                //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //return View(model);
                return BadRequest("Invalid login attempt.");
            }
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> Refresh(RefreshTokenRequetsDTO request)
        {
            var response = await _jwtAuthService.RefreshTokensAsync(request);
            if (response == null)
                return BadRequest("Invalid Token");
            return Ok(response);
        }

        //[HttpPost("LogOut")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return Ok("Logged out Successfully");
        //    //_logger.LogInformation(4, "User logged out.");
        //    //return RedirectToAction(nameof(HomeController.Index), "Home");
        //}

        [Authorize]
        [HttpGet("AuthTest")]
        public ActionResult<string> AuthTest()
        {
            return Ok("You are authenticated");
        }


        [Authorize]
        [HttpGet("Check")]
        public ActionResult<string> CheckIfSignedIn()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { signedIn = true, userId });
        }

        [HttpGet("ApiHelloWorld")]
        public string ApiHelloWorld()
        {
            return "Hello world";
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
