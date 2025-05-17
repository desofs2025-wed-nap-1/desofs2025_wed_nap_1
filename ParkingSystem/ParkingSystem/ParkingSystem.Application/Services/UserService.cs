using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Interfaces;

namespace ParkingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

    }
}
