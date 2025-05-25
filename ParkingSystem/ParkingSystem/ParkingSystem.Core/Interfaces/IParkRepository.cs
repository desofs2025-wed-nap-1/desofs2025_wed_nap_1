using ParkingSystem.Core.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParkingSystem.Core.Interfaces
{
    public interface IParkRepository
    {
        Task<Park?> AddPark(Park park);
        Task<Park?> UpdatePark(Park park, long parkId);
        Task<Park?> DeletePark(long id);
        Task<IEnumerable<Park>> GetAvailableParks();
    }
}
