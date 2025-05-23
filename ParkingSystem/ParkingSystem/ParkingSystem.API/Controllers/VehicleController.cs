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
        public IActionResult AddVehicle(VehicleDTO vehicleDto)
        {
            var result =_vehicleService.AddVehicleToUser(vehicleDto);
            if (result != null)
            {
                return Ok("Vehicle created successfully.");
            }
            return BadRequest("Failed to create Vehicle.");
        }

        [HttpPut("update")]
        public IActionResult UpdateVehicle(VehicleDTO vehicleDto)
        {
            var result = _vehicleService.UpdateVehicle(vehicleDto);
            if (result != null)
            {
                return Ok("Vehicle updated successfully.");
            }
            return NotFound("Failed to update vehicle.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteVehicle(long id)
        {
            var result = _vehicleService.DeleteVehicle(id);
            if (result != null)
            {
                return Ok("Vehicle deleted successfully.");
            }
            return NotFound("Vehicle not found.");
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetVehiclesByUser(long userId)
        {
            var vehicles = _vehicleService.GetVehiclesByUser(userId);
            return Ok(vehicles);
        }
    }
}
