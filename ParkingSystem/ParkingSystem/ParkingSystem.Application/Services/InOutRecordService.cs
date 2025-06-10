using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Application.Mappers;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;

namespace ParkingSystem.Application.Services
{
  public class InOutRecordService : IInOutRecordService
  {
    private readonly IInOutRecordRepository _repository;
    private readonly IParkRepository _parkRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<InOutRecordService> _logger;

    public InOutRecordService(IInOutRecordRepository repository, IParkRepository parkRepository,
      IVehicleRepository vehicleRepository, ILogger<InOutRecordService> logger)
    {
      _repository = repository;
      _logger = logger;
      _parkRepository = parkRepository;
      _vehicleRepository = vehicleRepository;
    }

    public async Task<InOutRecordDTO.InOutRecordResponseDto> CreateInOutRecordAsync(
      InOutRecordDTO.InOutRecordRequestDto dto)
    {
      _logger.LogInformation("Starting CreateInOutRecordAsync for Vehicle: {VehicleId}, Park: {ParkId}", dto.Vehicle, dto.Park);

      var park = await _parkRepository.GetParkById(dto.Park);
      if (park == null)
      {
        _logger.LogWarning("Park not found for ID: {ParkId}", dto.Park);
        throw new ArgumentException("Park not found");
      }

      var vehicle = await _vehicleRepository.FindById(dto.Vehicle);
      if (vehicle == null)
      {
        _logger.LogWarning("Vehicle not found for ID: {VehicleId}", dto.Vehicle);
        throw new ArgumentException("Vehicle not found");
      }

      var record = new InOutRecord(vehicle, park);

      var savedRecord = await _repository.AddInOutRecordAsync(record);

      _logger.LogInformation("InOutRecord created successfully with ID: {RecordId}", savedRecord.Id);

      return InOutRecordMapper.ToDto(savedRecord);
    }

    public async Task<IEnumerable<InOutRecordDTO.InOutRecordResponseDto>> GetByParkingAsync(int parkingId)
    {
      _logger.LogInformation("Fetching InOutRecords for Park ID: {ParkingId}", parkingId);

      var records = await _repository.GetInOutRecordsAsync(parkingId);

      _logger.LogInformation("Retrieved {Count} records for Park ID: {ParkingId}", records.Count(), parkingId);

      return records.Select(r => InOutRecordMapper.ToDto(r));
    }

    public async Task<InOutRecordDTO.InOutRecordResponseDto?> RegisterExitAsync(
      InOutRecordDTO.InOutRecordRequestDto dto)
    {
      _logger.LogInformation("Starting RegisterExitAsync for Vehicle: {VehicleId}, Park: {ParkId}", dto.Vehicle, dto.Park);

      var record = await _repository.GetInOutRecordsWithoutExit(dto.Vehicle, dto.Park);

      if (record == null)
      {
        _logger.LogWarning("No active InOutRecord found for Vehicle: {VehicleId} in Park: {ParkId}", dto.Vehicle, dto.Park);
        throw new ArgumentException("Record not found");
      }

      record.RegisterSaida();
      await _repository.UpdateInOutRecordAsync(record);

      _logger.LogInformation("Exit registered successfully for Record ID: {RecordId}", record.Id);

      return InOutRecordMapper.ToDto(record);
    }

    public async Task<string> GenerateCsvReportForParkingAsync(int parkingId)
    {
      var records = await _repository.GetInOutRecordsAsync(parkingId);

      if (records == null || !records.Any())
        throw new Exception($"No records found for parking lot with ID {parkingId}");

      var fileName = $"ParkingReport_{parkingId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
      var filePath = Path.Combine("Reports", fileName);
      Directory.CreateDirectory("Reports");

      using (var writer = new StreamWriter(filePath))
      {
        await writer.WriteLineAsync("Id,VehiclePlate,ParkName,DataEntrada,DataSaida");
        var header = "Id,VehiclePlate,ParkName,DataEntrada,DataSaida";
        await writer.WriteLineAsync(header);
        foreach (var record in records)
        {
          var line = $"{record.Id},{record.Vehicle.licensePlate},{record.Park.name},{record.DataEntrada:yyyy-MM-dd HH:mm:ss},{(record.DataSaida == DateTime.MinValue ? record.DataSaida.ToString("yyyy-MM-dd HH:mm:ss") : "")}";
          await writer.WriteLineAsync(line);
        }
      }

      _logger.LogInformation("CSV report generated at {FilePath}", Path.GetFullPath(filePath));
      return filePath;
    }

  }
}
