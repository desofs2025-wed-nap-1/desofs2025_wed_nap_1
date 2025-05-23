namespace ParkingSystem.Core.Aggregates
{
    public class Park 
    {
        public long Id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public int capacity { get; set; } = 0;
        public bool gateOpen { get; set; } = false;
    }
}