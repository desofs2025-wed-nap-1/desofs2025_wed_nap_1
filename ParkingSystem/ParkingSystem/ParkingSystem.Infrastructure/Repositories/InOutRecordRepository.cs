using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Infrastructure.Data;

namespace ParkingSystem.Infrastructure.Repositories
{
  public class InOutRecordRepository : IInOutRecordRepository
  {
    private readonly AppDbContext _context;

    public InOutRecordRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<InOutRecord> AddInOutRecordAsync(InOutRecord record)
    {
      _context.InOutRecords.Add(record);
      await _context.SaveChangesAsync();
      return record;
    }

    public async Task<InOutRecord> UpdateInOutRecordAsync(InOutRecord record)
    {
      _context.InOutRecords.Update(record);
      await _context.SaveChangesAsync();
      return record;
    }

    public async Task<InOutRecord> GetInOutRecordsWithoutExit(long vehicle, long parkingId)
    {
      return await _context.InOutRecords
        .Where(r => r.VehicleId == vehicle && r.DataSaida == DateTime.MinValue && r.ParkId == parkingId)
        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<InOutRecord>> GetInOutRecordsAsync(long parkingId)
    {
      return await _context.InOutRecords
        .Where(r => r.ParkId == parkingId)
        .ToListAsync();
    }

  }
}


