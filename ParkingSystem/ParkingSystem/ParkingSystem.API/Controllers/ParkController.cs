using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;
using System.Threading.Tasks;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkController : ControllerBase
    {
        private readonly IParkService _parkService;
        private readonly ILogger<ParkController> _logger;

        public ParkController(IParkService parkService, ILogger<ParkController> logger)
        {
            _parkService = parkService;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult AddPark(ParkDTO parkDto)
        {
            var result = _parkService.AddPark(parkDto);
            if (result != null)
            {
                return Ok("Park created successfully.");
            }
            return BadRequest("Failed to create park.");
        }

        [HttpPut("update")]
        public IActionResult UpdatePark(ParkDTO parkDto)
        {
            var result = _parkService.UpdatePark(parkDto);
            if (result != null)
            {
                return Ok("Park updated successfully.");
            }
            return BadRequest("Failed to update park.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeletePark(long id)
        {
            var result = _parkService.DeletePark(id);
            if (result != null)
            {
                return Ok("Park deleted successfully.");
            }
            return NotFound("Park not found.");
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableParks()
        {
            try
            {
                var parks = await _parkService.GetAvailableParks();
                _logger.LogInformation("Successfully gathered available parks");
                return Ok(parks);
            }
            catch (Exception e)
            {
                _logger.LogError("Error listing available parks: " + e.Message);
                return BadRequest("Error listing available parks");
            }
            
        }
    }
}