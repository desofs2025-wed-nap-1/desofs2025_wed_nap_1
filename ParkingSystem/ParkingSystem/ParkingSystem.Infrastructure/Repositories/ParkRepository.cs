using Microsoft.EntityFrameworkCore;
using ParkingSystem.Application.Exceptions;
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
            _context.Parks.Add(park);
            await _context.SaveChangesAsync();
            return park;
        }

        public async Task<Park?> UpdatePark(Park park)
        {
            var existingPark = await _context.Parks.FindAsync(park.Id);
            if (existingPark != null)
            {
                _context.Entry(existingPark).CurrentValues.SetValues(park);
                await _context.SaveChangesAsync();
                return existingPark;
            }
            throw new ParkNotFoundException($"Park with id {park.Id} was not found");
        }

        public async Task<Park?> DeletePark(long id)
        {
            var park = await _context.Parks.FindAsync(id);
            if (park != null)
            {
                _context.Parks.Remove(park);
                await _context.SaveChangesAsync();
                return park;
            }
            throw new ParkNotFoundException($"Park with id {id} was not found");
        }

        public async Task<IEnumerable<Park>> GetAvailableParks()
        {
            return await _context.Parks.Where(p => p.gateOpen).ToListAsync();
        }

        public async Task<Park?> SetGateStatus(long parkId, bool isOpen)
        {
            var park = await _context.Parks.FindAsync(parkId);
            if (park == null) return null;

            park.gateOpen = isOpen;
            await _context.SaveChangesAsync();
            return park;
        }

    }
}
