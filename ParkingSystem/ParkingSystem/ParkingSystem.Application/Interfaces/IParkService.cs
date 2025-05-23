using ParkingSystem.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ParkingSystem.Application.Interfaces
{
    public interface IParkService
    {
        Task<ParkDTO?> AddPark(ParkDTO parkDto);
        Task<ParkDTO?> UpdatePark(ParkDTO parkDto);
        Task<ParkDTO?> DeletePark(long id);
        Task<IEnumerable<ParkDTO>> GetAvailableParks();
    }
}