using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkController : ControllerBase
    {
        private readonly IParkService _parkService;

        public ParkController(IParkService parkService)
        {
            _parkService = parkService;
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
        public IActionResult GetAvailableParks()
        {
            var parks = _parkService.GetAvailableParks();
            return Ok(parks);
        }
    }
}