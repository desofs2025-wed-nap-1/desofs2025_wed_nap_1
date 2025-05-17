namespace ParkingSystem.Core.Aggregates
{
    public class ParkingRequest
    {
        public DateTime requestDate { get; set; }
        public int requestPeriod { get; set; }
        public State state { get; set; }
    }
}