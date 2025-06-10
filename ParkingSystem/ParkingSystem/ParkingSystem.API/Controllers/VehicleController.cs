using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;
using ParkingSystem.Core.Constants;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> AddVehicleAsync(VehicleDTO vehicleDto)
        {
            try
            {
                var result = await _vehicleService.AddVehicleToUser(vehicleDto);
                if (result != null)
                {
                    return Ok("Vehicle created successfully.");
                }
                return BadRequest("Failed to create Vehicle.");
            }
            catch
            {
                return StatusCode(500, "Internal error when adding vehicle, please try again later");
            }
            
        }

        [HttpPut("update")]
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> UpdateVehicle(VehicleDTO vehicleDto)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicle(vehicleDto);
                if (result != null)
                {
                    return Ok("Vehicle updated successfully.");
                }
                return NotFound("Failed to update vehicle.");
            }
            catch
            {
                return StatusCode(500, "Internal error when updating vehicle, please try again later");
            }
            
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> DeleteVehicle(long id)
        {
            try
            {
                var result = await _vehicleService.DeleteVehicle(id);
                if (result != null)
                {
                    return Ok("Vehicle deleted successfully.");
                }
                return NotFound("Vehicle not found.");
            }
            catch
            {
                return StatusCode(500, "Internal error when deleting vehicle, please try again later");
            }
            
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> GetVehiclesByUser(long userId)
        {
            try
            {
                var vehicles = await _vehicleService.GetVehiclesByUser(userId);
                return Ok(vehicles);
            }
            catch
            {
                return StatusCode(500, "Internal error when deleting vehicle, please try again later");
            }
            
        }
    }
}
