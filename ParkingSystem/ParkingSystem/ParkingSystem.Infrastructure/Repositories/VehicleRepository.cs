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
            var user = await _context.users.SingleOrDefaultAsync(u => u.username == username);
            if (user != null)
            {
                vehicle.UserId = user.Id; 
                _context.vehicle.Add(vehicle);
                await _context.SaveChangesAsync();
                return vehicle;
            }
            return null;
        }

        public async Task<Vehicle?> UpdateVehicle(Vehicle vehicle, long vehicleId)
        {
            var existingVehicle = await _context.vehicle.FindAsync(vehicleId);
            if (existingVehicle != null)
            {                
                existingVehicle.licensePlate = vehicle.licensePlate;
                existingVehicle.brand = vehicle.brand;
                existingVehicle.model = vehicle.model;
                existingVehicle.approved = vehicle.approved;
                await _context.SaveChangesAsync();
                return existingVehicle;
            }
            return null;
        }

        public async Task<Vehicle?> DeleteVehicle(long id)
        {
            var vehicle = await _context.vehicle.FindAsync(id);
            if (vehicle != null)
            {
                _context.vehicle.Remove(vehicle);
                await _context.SaveChangesAsync();
                return vehicle;
            }
            return null;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesFromUser(long userId)
        {
            return await _context.vehicle.Where(v => v.UserId == userId).ToListAsync();
        }
    }
}