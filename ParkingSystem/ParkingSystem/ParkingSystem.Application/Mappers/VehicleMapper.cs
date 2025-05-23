using ParkingSystem.Core.Aggregates;
using ParkingSystem.Application.DTOs;

namespace ParkingSystem.Application.Mappers
{
    public static class VehicleMapper
    {
        public static Vehicle ToVehicleDomain(VehicleDTO dto)
        {
            return new Vehicle
            {
                licensePlate = dto.licensePlate,
                brand = dto.brand,
                model = dto.model,
                approved = dto.approved
            };
        }

        public static VehicleDTO ToVehicleDto(Vehicle vehicle)
        {
            return new VehicleDTO
            {
                licensePlate = vehicle.licensePlate,
                brand = vehicle.brand,
                model = vehicle.model,
                approved = vehicle.approved
            };
        }
    }
}
