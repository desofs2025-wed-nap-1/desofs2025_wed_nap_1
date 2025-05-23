using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Application.DTOs
{
    public class UserDTO
    {
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }
}