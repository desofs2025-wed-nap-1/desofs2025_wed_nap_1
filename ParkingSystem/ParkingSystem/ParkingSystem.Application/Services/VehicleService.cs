using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Interfaces;

namespace ParkingSystem.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        
    }
}
