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
        public async Task<IActionResult> AddPark(ParkDTO parkDto)
        {
            var result = await _parkService.AddPark(parkDto);
            if (result != null)
            {
                return Ok("Park created successfully.");
            }
            return BadRequest("Failed to create park.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePark(ParkDTO parkDto, long parkId)
        {
            var result = await _parkService.UpdatePark(parkDto, parkId);
            if (result != null)
            {
                return Ok("Park updated successfully.");
            }
            return BadRequest("Failed to update park.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePark(long id)
        {
            var result = await _parkService.DeletePark(id);
            if (result != null)
            {
                return Ok("Park deleted successfully.");
            }
            return NotFound("Park not found.");
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableParks()
        {
            var parks = await _parkService.GetAvailableParks();
            return Ok(parks);
        }
    }
}