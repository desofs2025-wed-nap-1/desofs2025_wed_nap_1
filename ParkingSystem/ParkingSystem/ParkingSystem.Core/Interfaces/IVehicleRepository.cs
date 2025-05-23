using System.Threading.Tasks;
using System.Collections.Generic;
using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Core.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> AddVehicle(Vehicle vehicle, string username);
        Task<Vehicle?> UpdateVehicle(Vehicle vehicle);
        Task<Vehicle?> DeleteVehicle(long id);
        Task<IEnumerable<Vehicle>> GetAllVehiclesFromUser(long userId);
    }
}
