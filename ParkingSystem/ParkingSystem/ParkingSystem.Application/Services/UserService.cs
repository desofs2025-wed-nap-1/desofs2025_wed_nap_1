using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Mappers;
using System.Threading.Tasks;

namespace ParkingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SupabaseAuthService _authService;
        //private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, SupabaseAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
            //_tokenService = tokenService;
        }

        public async Task<UserDTO?> RegisterUser(UserDTO userDto)
        {
            if (await _userRepository.IsUsernameTaken(userDto.username))
            {
                throw new ArgumentException("Username already taken.");
            }

            // TODO: actually check how we could integrate this
            var userID = await _authService.CreateUserAsync(userDto.email, userDto.password, userDto.role);

            var user = UserMapper.ToUserDomain(userDto);

            var result = await _userRepository.AddUser(user);
            return result != null ? UserMapper.ToUserDto(result) : null;
        }

        public async Task<string?> Authenticate(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAndPassword(email, password);
            if (user != null)
            {
                return "token";
                //return _tokenService.GenerateToken(user);
            }
            return null;
        }

        public async Task<UserDTO?> UpdateUser(UserDTO userDto)
        {
            var user = UserMapper.ToUserDomain(userDto);
            var updatedUser = await _userRepository.UpdateUser(user);
            return updatedUser != null ? UserMapper.ToUserDto(updatedUser) : null;
        }

        public async Task<UserDTO?> DeleteUser(long id)
        {
            var deletedUser = await _userRepository.DeleteUser(id);
            return deletedUser != null ? UserMapper.ToUserDto(deletedUser) : null;
        }
    }
}
