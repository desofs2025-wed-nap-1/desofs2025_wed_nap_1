using ParkingSystem.Application.DTOs;
using System.Threading.Tasks;
namespace ParkingSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> RegisterUser(UserDTO userDto);
        Task<string?> Authenticate(string email, string password);
        Task<UserDTO?> UpdateUser(UserDTO userDto);
        Task<UserDTO?> DeleteUser(long id);
        Task<UserDTO?> RegisterParkManager(UserDTO userDto);
        Task<bool> ActivateSubscription(long userId, long parkId);
    }
}
