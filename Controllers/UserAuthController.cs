using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiteListing_Backend.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhiteListing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Supabase.Client _supabase;

        public UserAuthController(UserManager<ApplicationUser> userManager,
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
        public async Task<ActionResult> Login(RegistrationModel request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);
            //return (Ok(result));
            if (result.Succeeded)
            {
                return Ok("Successfully logged in");
            }
            return BadRequest("Error");
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
    }
}
