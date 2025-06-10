using ParkingSystem.Application.DTOs;
using ParkingSystem.Core.Aggregates;

namespace ParkingSystem.Application.Mappers
{
  public static class InOutRecordMapper
  {
    public static InOutRecordDTO.InOutRecordResponseDto ToDto(InOutRecord record)
    {
      return new InOutRecordDTO.InOutRecordResponseDto
      {
        Id = record.Id,
        Vehicle = record.Vehicle.Id,
        Park = record.Park.Id,
        DataEntrada = record.DataEntrada,
        DataSaida = record.DataSaida
      };
    }
  }
}
