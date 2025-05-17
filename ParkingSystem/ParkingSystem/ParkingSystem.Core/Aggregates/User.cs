using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Core.Aggregates
{
    public class User 
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }
        public Role role { get; set; }
    }
}