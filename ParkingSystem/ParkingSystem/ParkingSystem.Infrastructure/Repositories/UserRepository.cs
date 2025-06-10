using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Infrastructure.Data;
namespace ParkingSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUser(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }

        public async Task<User?> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }

        public async Task<User?> GetUserById(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.email == email && u.password == password);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.username == username);
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.Users.AnyAsync(u => u.username == username);
        }
    }
}
