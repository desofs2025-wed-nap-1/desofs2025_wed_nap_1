using ParkingSystem.Core.Aggregates;

namespace ParkingSystem.Core.Interfaces
{
  public interface IInOutRecordRepository
  {
    Task<IEnumerable<InOutRecord>> GetInOutRecordsAsync(long ParkingId);
    Task<InOutRecord> AddInOutRecordAsync(InOutRecord InOutRecord);
    Task<InOutRecord> UpdateInOutRecordAsync(InOutRecord InOutRecord);
    Task<InOutRecord> GetInOutRecordsWithoutExit(long vehicle, long park);
  }
}

