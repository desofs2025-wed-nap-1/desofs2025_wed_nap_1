using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            var userId = User?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            return Ok(new
            {
                message = "Protected endpoint accessed",
                userId = userId
            });
        }


        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Public endpoint, no authentication required");
        }
    }
}
