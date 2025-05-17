/*namespace ParkingSystem.API.Controllers
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
            _vehicleService.AddVehicle(vehicleDto);
            return Ok("Vehicle added successfully.");
        }

        [HttpPut("update")]
        public IActionResult UpdateVehicle(VehicleDTO vehicleDto)
        {
            _vehicleService.UpdateVehicle(vehicleDto);
            return Ok("Vehicle updated successfully.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteVehicle(Guid id)
        {
            _vehicleService.DeleteVehicle(id);
            return Ok("Vehicle deleted.");
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetVehiclesByUser(Guid userId)
        {
            var vehicles = _vehicleService.GetVehiclesByUser(userId);
            return Ok(vehicles);
        }
    }
}*/
