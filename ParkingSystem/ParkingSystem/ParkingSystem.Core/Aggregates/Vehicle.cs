namespace ParkingSystem.Core.Aggregates
{
    public class Vehicle 
    {
        public string licensePlate { get; set; } = string.Empty;
        public string brand { get; set; } = string.Empty;
        public string model { get; set; } = string.Empty;
        public bool approved { get; set; } = false;
    }
}