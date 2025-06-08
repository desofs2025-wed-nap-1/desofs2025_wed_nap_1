using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Constants;
using ParkingSystem.Application.Exceptions;

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
        [Authorize(Roles = RoleNames.ParkManager)]
        public async Task<IActionResult> AddPark(ParkDTO parkDto)
        {
            try
            {
                var result = await _parkService.AddPark(parkDto);
                if (result != null)
                {
                    return Ok("Park created successfully.");
                }
                return BadRequest("Failed to create park.");
            }
            catch
            {
                return BadRequest("Failed to add park");
            }

        }

        [HttpPut("update")]
        [Authorize(Roles = RoleNames.ParkManager)]
        public async Task<IActionResult> UpdatePark(ParkDTO parkDto)
        {
            try
            {
                var result = await _parkService.UpdatePark(parkDto);
                if (result != null)
                {
                    return Ok("Park updated successfully.");
                }
                return BadRequest("Failed to update park, ensure the request contains valid information.");
            }
            catch (ParkNotFoundException)
            {
                return NotFound("The Park provided for update was not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error when updating Park");
            }
            
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = RoleNames.ParkManager)]
        public async Task<IActionResult> DeletePark(long id)
        {
            try
            {
                var result = await _parkService.DeletePark(id);
                if (result != null)
                {
                    return Ok("Park deleted successfully.");
                }
                return BadRequest("Failed to delete park, ensure the request contains valid information"); 
            }
            catch (ParkNotFoundException)
            {
                return NotFound("The Park provided for deletion was not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error when deleting Park");
            }
        }

        [HttpGet("available")]
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> GetAvailableParks()
        {
            try
            {
                var parks = await _parkService.GetAvailableParks();
                return Ok(parks);
            }
            catch (Exception)
            {
                return BadRequest("Error listing available parks");
            }

        }
    }
}