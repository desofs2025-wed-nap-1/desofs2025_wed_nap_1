using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Mappers;
using System.Threading.Tasks;
using ParkingSystem.Application.Exceptions;
using Supabase.Storage;

namespace ParkingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IParkRepository _parkRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IParkRepository parkRepository, ISubscriptionRepository subscriptionRepository,
        IAuthenticationService authService, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _parkRepository = parkRepository;
            _subscriptionRepository = subscriptionRepository;
            _authService = authService;
            _logger = logger;
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
            try
            {
                var findUser = await _userRepository.GetUserByUsername(userDto.username);
                if (findUser == null)
                {
                    // don't log here, because it's logged below in the "catch" block
                    throw new UserNotFoundException("A user with the provided details was not found");
                }
                _logger.LogInformation("Provided user exists and is valid, will update: " + findUser.Id);
                var user = UserMapper.ToUserDomainWithId(userDto, findUser.Id);
                var updatedUser = await _userRepository.UpdateUser(user);
                if (updatedUser != null)
                {
                    return UserMapper.ToUserDto(updatedUser);
                }
                throw new Exception("UserRepository returned null after update");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating user: " + ex.Message);
                throw;
            }

        }

        public async Task<UserDTO?> DeleteUser(long id)
        {
            try
            {
                var deletedUser = await _userRepository.DeleteUser(id);
                if (deletedUser != null)
                {
                    return UserMapper.ToUserDto(deletedUser);
                }
                throw new Exception("UserRepository returned null after delete");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting user: " + ex.Message);
                throw;
            }

        }

        public async Task<UserDTO?> RegisterParkManager(UserDTO userDto)
        {
            if (await _userRepository.IsUsernameTaken(userDto.username))
            {
                throw new ArgumentException("Username already taken.");
            }

            var userID = await _authService.CreateUserAsync(userDto.email, userDto.password, userDto.role);

            var user = UserMapper.ToUserDomain(userDto);
            var result = await _userRepository.AddUser(user);
            return result != null ? UserMapper.ToUserDto(result) : null;
        }

        public async Task<bool> ActivateSubscription(long userId, long parkId)
        {
            try
            {
                var park = await _parkRepository.GetParkById(parkId);
                var user = await _userRepository.GetUserById(userId);
                if (park == null || user == null)
                {
                    _logger.LogInformation("Error finding parkid " + parkId + " or userid " + userId);
                    return false;
                }
                var existingSubscription = await _subscriptionRepository.GetActiveSubscription(userId, parkId);
                if (existingSubscription != null)
                {
                    _logger.LogInformation($"Subscription already active for userid {userId} and parkid {parkId}.");
                    return false;
                }
                var success = await _subscriptionRepository.ActivateSubscription(userId, parkId);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error activating subscription: " + ex.Message);
                return false;
            }
        }

    }
}
