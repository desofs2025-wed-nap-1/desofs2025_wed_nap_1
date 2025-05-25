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
                role_id = dto.role_id,
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
                role_id = user.role_id
            };
        }
    }
}
