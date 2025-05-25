using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Infrastructure.Data;
namespace ParkingSystem.Infrastructure.Repositories
{
    public class ParkRepository : IParkRepository
    {
        private readonly AppDbContext _context;

        public ParkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Park?> AddPark(Park park)
        {
            _context.park.Add(park);
            await _context.SaveChangesAsync();
            return park;
        }

        public async Task<Park?> UpdatePark(Park park, long parkId)
        {
            var existingPark = await _context.park.FindAsync(parkId);
            if (existingPark != null)
            {
                existingPark.name = park.name;
                existingPark.location = park.location;
                existingPark.capacity = park.capacity;
                existingPark.gateOpen = park.gateOpen;
                await _context.SaveChangesAsync();
                return existingPark;
            }
            return null;
        }

        public async Task<Park?> DeletePark(long id)
        {
            var park = await _context.park.FindAsync(id);
            if (park != null)
            {
                _context.park.Remove(park);
                await _context.SaveChangesAsync();
                return park;
            }
            return null;
        }

        public async Task<IEnumerable<Park>> GetAvailableParks()
        {
            return await _context.park.Where(p => p.gateOpen).ToListAsync();
        }
    }
}
