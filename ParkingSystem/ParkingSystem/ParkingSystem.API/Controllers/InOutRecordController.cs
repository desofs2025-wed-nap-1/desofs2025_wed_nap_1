using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Application.Services;
using ParkingSystem.Core.Constants;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InOutRecordController : ControllerBase
    {
        private readonly IInOutRecordService _service;
        private readonly ILogger<InOutRecordController> _logger;

        public InOutRecordController(IInOutRecordService service, ILogger<InOutRecordController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Registers a vehicle entry in a parking lot.
        /// </summary>
        [HttpPost("entry")]
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> RegisterEntry([FromBody] InOutRecordDTO.InOutRecordRequestDto dto)
        {
            try
            {
                var result = await _service.CreateInOutRecordAsync(dto);
                return CreatedAtAction(nameof(GetByParking), new { parkingId = result.Park }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for RegisterEntry");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterEntry");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Registers the vehicle exit.
        /// </summary>
        [HttpPut("exit")]
        [Authorize(Roles = RoleNames.Client)]
        public async Task<IActionResult> RegisterExit([FromBody] InOutRecordDTO.InOutRecordRequestDto dto)
        {
            try
            {
                var result = await _service.RegisterExitAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for RegisterExit");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterExit");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Gets all records by parking ID.
        /// </summary>
        [HttpGet("by-park/{parkingId:int}")]
        [Authorize(Roles = RoleNames.ParkManager)]
        public async Task<IActionResult> GetByParking(int parkingId)
        {
            var records = await _service.GetByParkingAsync(parkingId);
            return Ok(records);
        }

        [HttpGet("{parkId}")]
        [Authorize(Roles = RoleNames.ParkManager)]
        public async Task<IActionResult> GenerateReport(int parkId)
        {
          try
          {
            var filePath = await _service.GenerateCsvReportForParkingAsync(parkId);
            return Ok(new { file = filePath, message = "CSV report generated successfully." });
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "Error generating CSV report for ParkId {ParkId}", parkId);
            return StatusCode(500, "Error generating CSV report.");
          }
        }
    }
}
