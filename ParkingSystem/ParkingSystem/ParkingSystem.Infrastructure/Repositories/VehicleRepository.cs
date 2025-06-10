using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Infrastructure.Data;
namespace ParkingSystem.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;

        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> AddVehicle(Vehicle vehicle, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.username == username);
            if (user != null)
            {
                vehicle.UserId = user.Id;
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
                return vehicle;
            }
            return null;
        }

        public async Task<Vehicle?> UpdateVehicle(Vehicle vehicle)
        {
            var existingVehicle = await _context.Vehicles.FindAsync(vehicle.licensePlate);
            if (existingVehicle != null)
            {
                _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
                await _context.SaveChangesAsync();
                return existingVehicle;
            }
            return null;
        }

        public async Task<Vehicle?> DeleteVehicle(long id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                return vehicle;
            }
            return null;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesFromUser(long userId)
        {
            return await _context.Vehicles.Where(v => v.UserId == userId).ToListAsync();
        }

        public async Task<Vehicle?> FindById(long id)
        {
          return await _context.Vehicles.FindAsync(id);
        }
    }
}
