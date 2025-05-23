using ParkingSystem.Core.Aggregates;
using System.Threading.Tasks;
namespace ParkingSystem.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AddUser(User user);
        Task<User?> UpdateUser(User user);
        Task<User?> DeleteUser(long id);
        Task<User?> GetUserById(long id);
        Task<User?> GetUserByEmailAndPassword(string email, string password);    
        Task<bool> IsUsernameTaken(string username);
    }
}
