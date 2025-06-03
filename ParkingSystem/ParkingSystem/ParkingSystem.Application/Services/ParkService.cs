using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Mappers;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ParkingSystem.Application.Services
{
    public class ParkService : IParkService
    {
        private readonly IParkRepository _parkRepository;
        private readonly ILogger<ParkService> _logger;

        public ParkService(IParkRepository parkRepository, ILogger<ParkService> logger)
        {
            _parkRepository = parkRepository;
            _logger = logger;
        }

        public async Task<ParkDTO?> AddPark(ParkDTO parkDto)
        {
            _logger.LogInformation("Adding park {name}", parkDto.name);
            try
            {
                var park = ParkMapper.ToParkDomain(parkDto);
                var result = await _parkRepository.AddPark(park);
                if (result == null)
                {
                    _logger.LogWarning("Failed to add Park. Repository returned null");
                    return null;
                }
                _logger.LogInformation("Successfully added park {name}", parkDto.name);
                return ParkMapper.ToParkDto(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception adding park: " + ex.Message);
                throw;
            }
        }

        public async Task<ParkDTO?> UpdatePark(ParkDTO parkDto)
        {
            var park = ParkMapper.ToParkDomain(parkDto);
            var updatedPark = await _parkRepository.UpdatePark(park);
            return updatedPark != null ? ParkMapper.ToParkDto(updatedPark) : null;
        }

        public async Task<ParkDTO?> DeletePark(long id)
        {
            var deletedPark = await _parkRepository.DeletePark(id);
            return deletedPark != null ? ParkMapper.ToParkDto(deletedPark) : null;
        }

        public async Task<IEnumerable<ParkDTO>> GetAvailableParks()
        {
            _logger.LogInformation("Fetching available parks");
            try
            {
                var parks = await _parkRepository.GetAvailableParks();
                var parkDtos = new List<ParkDTO>();
                foreach (var park in parks)
                {
                    parkDtos.Add(ParkMapper.ToParkDto(park));
                }
                _logger.LogInformation("Found {Count} available parks", parkDtos.Count);
                return parkDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching Parks: " + ex.Message);
                // preserve the exception after logging - it's caught on the controller for return purposes
                throw;
            }
            
        }
    }
}
