using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Mappers;
using System.Net.Mail;
namespace ParkingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        //private readonly ITokenService _tokenService; 

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            //_tokenService = tokenService;
        }

        public async Task<UserDTO?> RegisterUser(UserDTO userDto)
        {
            /*if (!IsValidEmail(userDto.email))
            {
                throw new ArgumentException("Invalid email format.");
            }*/
            if (await _userRepository.IsEmailTaken(userDto.email))
            {
                throw new ArgumentException("Email already registered.");
            }
            if (await _userRepository.IsUsernameTaken(userDto.username))
            {
                throw new ArgumentException("Username already taken.");
            }
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

        public async Task<UserDTO?> UpdateUser(UserDTO userDto, long userId)
        {
            var user = UserMapper.ToUserDomain(userDto);
            var updatedUser = await _userRepository.UpdateUser(user, userId);
            return updatedUser != null ? UserMapper.ToUserDto(updatedUser) : null;
        }

        public async Task<UserDTO?> DeleteUser(long id)
        {
            var deletedUser = await _userRepository.DeleteUser(id);
            return deletedUser != null ? UserMapper.ToUserDto(deletedUser) : null;
        }
        /*private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                throw new ArgumentException("Invalid email format.");
            }
        }*/
    }
}
