using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Infrastructure.Data;
using BCrypt.Net;
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
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
            user.password = hashedPassword;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUser(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
                user.password = hashedPassword;
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
            var user = await _context.Users.SingleOrDefaultAsync(u => u.email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.Users.AnyAsync(u => u.username == username);
        }
        
        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Users.AnyAsync(u => u.email == email);
        }
    }
}
