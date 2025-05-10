using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        //private readonly ILogger _logger;

        public UserAuthController(CustomUserManager userManager,
            SignInManager<ApplicationUser> signInManager, Supabase.Client supabase)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _supabase = supabase;
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
        public async Task<ActionResult> Login(LoginModel request)
        {

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);
            if (result == null)
            {
                return BadRequest("Invalid Username or Password.");
            }
            if (result.Succeeded)
            {
                //_logger.LogInformation(1, "User logged in.");
                //return RedirectToLocal(returnUrl);
                return Ok("Success");
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
        // GET: api/<UserAuthController>
        [HttpGet]
        public string Get()
        {
            return "Hello World";
        }

        // GET api/<UserAuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserAuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserAuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserAuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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
