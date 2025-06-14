namespace ParkingSystem.Core.Aggregates
{
    public class Subscription
    {
        public long Id { get; set; }
        public long userId { get; set; }
        public User User { get; private set; }
        public long parkId { get; set; }
        public Park Park { get; private set; }
        public bool isActive { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}