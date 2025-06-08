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
                role = Enum.TryParse(dto.role, out Role role) ? role : 0
            };
        }

        public static User ToUserDomainWithId(UserDTO dto, long id)
        {
            return new User
            {
                Id = id,
                username = dto.username,
                email = dto.email,
                password = dto.password,
                phoneNumber = dto.phoneNumber,
                role = Enum.TryParse(dto.role, out Role role) ? role : 0
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
                role = user.role.ToString()
            };
        }
    }
}
