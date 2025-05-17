namespace ParkingSystem.Application.DTOs
{
    public class ParkDTO
    {
        public string name { get; set; }
        public string location { get; set; }
        public int capacity { get; set; }
        public bool gateOpen { get; set; }
    }
}