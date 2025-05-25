using ParkingSystem.Core.Aggregates;
using ParkingSystem.Application.DTOs;

namespace ParkingSystem.Application.Mappers
{
    public static class UserMapper
    {
        public static User ToUserDomain(UserDTO dto)
        {
            return new User
            {
                username = dto.username,
                email = dto.email,
                password = dto.password,
                phoneNumber = dto.phoneNumber,
                role = Enum.IsDefined(typeof(Role), dto.role) ? (Role)dto.role : 0
            };
        }

        public static UserDTO ToUserDto(User user)
        {
            return new UserDTO
            {
                username = user.username,
                email = user.email,
                phoneNumber = user.phoneNumber,
                password = user.password,
                role = (int)user.role
            };
        }
    }
}
