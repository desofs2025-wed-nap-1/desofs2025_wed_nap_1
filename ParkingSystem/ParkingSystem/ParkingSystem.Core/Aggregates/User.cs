using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Core.Aggregates
{
    public class User 
    {
        public long Id { get; set; } = 0;
        public string username { get; set; } = string.Empty;
        public string email { get; set; }= string.Empty;
        public string password { get; set; }= string.Empty;
        public string phoneNumber { get; set; }= string.Empty;
        public Role role { get; set; }
    }
}