namespace ParkingSystem.Core.Aggregates
{
  public class InOutRecord
  {
    public long Id { get; }

    public long VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; }

    public long ParkId { get; private set; }
    public Park Park { get; private set; }

    public DateTime DataEntrada { get; private set; }
    public DateTime DataSaida { get; private set; }

    public InOutRecord(Vehicle vehicle, Park park)
    {
      Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
      VehicleId = vehicle.Id;

      Park = park ?? throw new ArgumentNullException(nameof(park));
      ParkId = park.Id;

      DataEntrada = DateTime.UtcNow;
      DataSaida = DateTime.MinValue;
    }

    public void RegisterSaida()
    {
      DataSaida = DateTime.UtcNow;
    }

    // Parameterless constructor for EF Core
    protected InOutRecord() { }
  }
}
