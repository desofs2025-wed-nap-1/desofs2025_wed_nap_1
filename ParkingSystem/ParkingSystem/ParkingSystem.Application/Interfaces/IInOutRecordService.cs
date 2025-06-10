using ParkingSystem.Application.DTOs;

namespace ParkingSystem.Application.Interfaces;

public interface IInOutRecordService
{
  public Task<InOutRecordDTO.InOutRecordResponseDto> CreateInOutRecordAsync(
    InOutRecordDTO.InOutRecordRequestDto dto);

  public Task<IEnumerable<InOutRecordDTO.InOutRecordResponseDto>> GetByParkingAsync(int parkingId);

  public Task<InOutRecordDTO.InOutRecordResponseDto?> RegisterExitAsync(
    InOutRecordDTO.InOutRecordRequestDto dto);

  public Task<string> GenerateCsvReportForParkingAsync(int parkingId);
}
