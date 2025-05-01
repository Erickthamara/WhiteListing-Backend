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

        public UserAuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login(RegistrationModel request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);
            return (Ok("Successfully logged in."));
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
