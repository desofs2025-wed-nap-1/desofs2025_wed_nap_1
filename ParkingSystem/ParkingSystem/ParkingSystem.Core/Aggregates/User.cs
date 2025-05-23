using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Core.Aggregates
{
    public class User 
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; }= string.Empty;
        public string Password { get; set; }= string.Empty;
        public string PhoneNumber { get; set; }= string.Empty;
        public Role Role { get; set; }

        public User(string username, string email, string password, string phoneNumber, Role role)
        {
            Username = username;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            Role = role;
        }
        public User(){}
    }
}