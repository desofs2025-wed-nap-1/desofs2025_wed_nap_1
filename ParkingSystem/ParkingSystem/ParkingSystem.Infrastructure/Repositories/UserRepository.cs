using ParkingSystem.Core.Interfaces;
using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>
                {
                    new User()
                };

            return await Task.FromResult(users);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var user = new User();
            return await Task.FromResult(user);
        }
    }
}