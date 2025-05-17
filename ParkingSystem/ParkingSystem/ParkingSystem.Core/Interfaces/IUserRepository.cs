using ParkingSystem.Core.Aggregates;

namespace ParkingSystem.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
}