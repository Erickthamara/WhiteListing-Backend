using Microsoft.AspNetCore.Mvc;
using WhiteListing_Backend.SupabaseModels;

namespace WhiteListing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Supabase.Client _supabase;

        public HealthController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        [HttpGet]
        public async Task<IActionResult> CheckHealth()
        {
            try
            {
                await _supabase.From<SupabaseUserModel>().Get();
                return Ok(new { status = "healthy", supabase = "connected" });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { status = "unhealthy", error = ex.Message });
            }
        }
    }
}
