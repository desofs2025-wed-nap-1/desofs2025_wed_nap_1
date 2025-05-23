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
    }
}
