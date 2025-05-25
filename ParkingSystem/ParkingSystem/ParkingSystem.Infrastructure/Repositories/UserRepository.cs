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
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUser(User user, long userId)
        {
            var existingUser = await _context.users.FindAsync(userId);
            if (existingUser != null)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
                user.password = hashedPassword;
                existingUser.username = user.username;
                existingUser.email = user.email;
                existingUser.phoneNumber = user.phoneNumber;
                existingUser.role_id = user.role_id;
                await _context.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }

        public async Task<User?> DeleteUser(long id)
        {
            var user = await _context.users.FindAsync(id);
            if (user != null)
            {
                _context.users.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }

        public async Task<User?> GetUserById(long id)
        {
            return await _context.users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            var user = await _context.users.SingleOrDefaultAsync(u => u.email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.users.AnyAsync(u => u.username == username);
        }
        
        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.users.AnyAsync(u => u.email == email);
        }
    }
}
