using Microsoft.AspNetCore.Mvc;
using YourProject.DTOs;
using YourProject.Services;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;

        public AuthController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var result = await _authService.LoginWithSupabase(dto.Email, dto.Password);
                return Ok(new
                {
                    token = result.access_token,
                    user = result.user
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
