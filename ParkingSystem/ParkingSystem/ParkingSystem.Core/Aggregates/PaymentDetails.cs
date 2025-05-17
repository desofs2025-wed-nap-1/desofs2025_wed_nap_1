using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Core.Aggregates
{
    public class PaymentDetails 
    {
        public PaymentMethod type { get; set; }
        public string details { get; set; }
    }
}