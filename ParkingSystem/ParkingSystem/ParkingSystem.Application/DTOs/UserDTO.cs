using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Application.DTOs
{
    public class UserDTO
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }
        public Role role { get; set; }
    }
}