using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Interfaces;
namespace ParkingSystem.Application.Services
{
    public class ParkService : IParkService
    {
        private readonly IParkRepository _parkRepository;

        public ParkService(IParkRepository parkRepository)
        {
            _parkRepository = parkRepository;
        }

        
    }
}
