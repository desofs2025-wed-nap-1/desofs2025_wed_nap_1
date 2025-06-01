using  ParkingSystem.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ParkingSystem.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<VehicleDTO?> AddVehicleToUser(VehicleDTO vehicleDto);
        Task<VehicleDTO?> UpdateVehicle(VehicleDTO vehicleDto, long vehicleId);
        Task<VehicleDTO?> DeleteVehicle(long id);
        Task<IEnumerable<VehicleDTO>> GetVehiclesByUser(long userId);
    }
}