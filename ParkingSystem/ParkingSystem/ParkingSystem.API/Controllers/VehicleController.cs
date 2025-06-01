using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddVehicle(VehicleDTO vehicleDto)
        {
            var result = await _vehicleService.AddVehicleToUser(vehicleDto);
            if (result != null)
            {
                return Ok("Vehicle created successfully.");
            }
            return BadRequest("Failed to create Vehicle.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateVehicle(VehicleDTO vehicleDto, long vehicleId)
        {
            var result = await _vehicleService.UpdateVehicle(vehicleDto, vehicleId);
            if (result != null)
            {
                return Ok("Vehicle updated successfully.");
            }
            return NotFound("Failed to update vehicle.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(long id)
        {
            var result = await _vehicleService.DeleteVehicle(id);
            if (result != null)
            {
                return Ok("Vehicle deleted successfully.");
            }
            return NotFound("Vehicle not found.");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetVehiclesByUser(long userId)
        {
            var vehicles = await _vehicleService.GetVehiclesByUser(userId);
            return Ok(vehicles);
        }
    }
}
