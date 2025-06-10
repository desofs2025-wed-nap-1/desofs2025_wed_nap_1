using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Mappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using ParkingSystem.Application.Exceptions;
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
            _logger.LogInformation("Updating park {name}", parkDto.name);
            try
            {
                var park = ParkMapper.ToParkDomain(parkDto);
                var updatedPark = await _parkRepository.UpdatePark(park);

                if (updatedPark == null)
                {
                    _logger.LogWarning("Failed to update Park. Repository returned null");
                    return null;
                }
                _logger.LogInformation("Successfully updated park {name}", parkDto.name);
                return ParkMapper.ToParkDto(updatedPark);
            }
            catch (ParkNotFoundException pEx)
            {
                _logger.LogError(pEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception updating park: " + ex.Message);
                throw;
            }
            
        }

        public async Task<ParkDTO?> DeletePark(long id)
        {
            _logger.LogInformation("Deleting park with ID: {id}", id);
            try
            {
                var deletedPark = await _parkRepository.DeletePark(id);
                if (deletedPark == null)
                {
                    _logger.LogWarning("Failed to delete park. Repository returned null");
                    return null;
                }
                _logger.LogInformation("Successfully deleted park with ID {id}", id);
                return ParkMapper.ToParkDto(deletedPark);
            }
            catch (ParkNotFoundException pEx)
            {
                _logger.LogError(pEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting park: " + ex.Message);
                throw;
            }
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

        public async Task<ParkDTO?> SetGateStatus(long parkId, bool isOpen)
        {
            _logger.LogInformation("Setting gate status for park {parkId} to {isOpen}", parkId, isOpen);
            var result = await _parkRepository.SetGateStatus(parkId, isOpen);
            if (result == null)
            {
                _logger.LogWarning("Failed to set gate status: park not found");
                return null;
            }
            return ParkMapper.ToParkDto(result);
        }

    }
}
