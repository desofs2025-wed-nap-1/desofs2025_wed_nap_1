using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Interfaces;
using  ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Mappers;
using System.Text.RegularExpressions;
//using ParkingSystem.Core.Entities;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Application.Exceptions;
namespace ParkingSystem.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehicleService> _logger;
        private static List<string> listBrands = new List<string>
        {
            "Acura", "Alfa Romeo", "Aston Martin", "Audi", "Bentley", "BMW", "Bugatti",
            "Buick", "Cadillac", "Chevrolet", "Chrysler", "Citroën", "Dodge", "Ferrari",
            "Fiat", "Ford", "Genesis", "GMC", "Honda", "Hyundai", "Infiniti", "Jaguar",
            "Jeep", "Kia", "Lamborghini", "Land Rover", "Lexus", "Lincoln", "Lotus",
            "Maserati", "Mazda", "McLaren", "Mercedes-Benz", "Mini", "Mitsubishi",
            "Nissan", "Pagani", "Peugeot", "Porsche", "Ram", "Renault", "Rolls-Royce",
            "Saab", "Subaru", "Suzuki", "Tesla", "Toyota", "Volkswagen", "Volvo", "Opel"
        };

        public VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<VehicleDTO?> AddVehicleToUser(VehicleDTO vehicleDto)
        {
            try
            {
                _logger.LogInformation($"Adding vehicle {vehicleDto.licensePlate} to user {vehicleDto.username}");
                if (!IsValidLicensePlate(vehicleDto.licensePlate))
                {
                    throw new ArgumentException("License plate format is invalid.");
                }
                if (!IsValidBrand(vehicleDto.brand))
                {
                    throw new ArgumentException("Brand is invalid.");
                }
                var vehicle = VehicleMapper.ToVehicleDomain(vehicleDto);
                var result = await _vehicleRepository.AddVehicle(vehicle, vehicleDto.username);
                if (result == null)
                {
                    throw new UserNotFoundException($"Vehicle {vehicleDto.licensePlate} couldn't be added because user {vehicleDto.username} was not found");
                }
                else
                {
                    return VehicleMapper.ToVehicleDto(result);
                }

            }
            catch (ArgumentException argEx)
            {
                _logger.LogError($"Error adding vehicle {vehicleDto.licensePlate} - Invalid arguments were provided: {argEx.Message}");
                // Will be caught by the controller and return an error status to the client
                throw;
            }
            catch (UserNotFoundException usrEx)
            {
                _logger.LogError($"Error adding vehicle {vehicleDto.licensePlate} - Invalid user was provided: {usrEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding vehicle {vehicleDto.licensePlate}: {ex.Message}");
                throw;
            }
        }

        public async Task<VehicleDTO?> UpdateVehicle(VehicleDTO vehicleDto)
        {
            try
            {
                _logger.LogInformation($"Updating vehicle {vehicleDto.licensePlate} of user {vehicleDto.username}");
                if (!IsValidLicensePlate(vehicleDto.licensePlate))
                {
                    throw new ArgumentException("License plate format is invalid.");
                }
                if (!IsValidBrand(vehicleDto.brand))
                {
                    throw new ArgumentException("Brand is invalid.");
                }
                var vehicle = VehicleMapper.ToVehicleDomain(vehicleDto);
                var updatedVehicle = await _vehicleRepository.UpdateVehicle(vehicle);
                if (updatedVehicle == null)
                {
                    throw new UserNotFoundException($"Vehicle {vehicleDto.licensePlate} couldn't be updated because user {vehicleDto.username} was not found");
                }
                else
                {
                    return VehicleMapper.ToVehicleDto(updatedVehicle);
                }
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError($"Error adding vehicle {vehicleDto.licensePlate} - Invalid arguments were provided: {argEx.Message}");
                // Will be caught by the controller and return an error status to the client
                throw;
            }
            catch (UserNotFoundException usrEx)
            {
                _logger.LogError($"Error adding vehicle {vehicleDto.licensePlate} - Invalid user was provided: {usrEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding vehicle {vehicleDto.licensePlate}: {ex.Message}");
                throw;
            }
            
        }

        public async Task<VehicleDTO?> DeleteVehicle(long id)
        {
            try
            {
                var deletedVehicle = await _vehicleRepository.DeleteVehicle(id);
                if (deletedVehicle == null)
                {
                    throw new VehicleNotFoundException($"Vehicle with id {id} does not exist");
                }
                else
                {
                    return VehicleMapper.ToVehicleDto(deletedVehicle);
                }
            }
            catch (VehicleNotFoundException vEx)
            {
                _logger.LogError($"Error when deleting vehicle: {vEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error when deleting vehicle {ex.Message}");
                throw;
            }
            
        }

        public async Task<IEnumerable<VehicleDTO>> GetVehiclesByUser(long userId)
        {
            try
            {
                var result = await _vehicleRepository.GetAllVehiclesFromUser(userId);
                List<VehicleDTO> listVehicles = new List<VehicleDTO>();
                if (result.Count() == 0)
                {
                    _logger.LogWarning($"User {userId} has no Vehicles");
                    return listVehicles;
                }
                foreach (var item in result)
                {
                    listVehicles.Add(VehicleMapper.ToVehicleDto(item));
                }
                _logger.LogInformation($"A set of {listVehicles.Count()} was returned for User {userId}.");
                return listVehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error listing vehicles of user with id {userId}: {ex.Message}");
                throw;
            }
            
        }

        private static bool IsValidLicensePlate(string licensePlate)
        {
            var regex = new Regex(@"^(([A-Z]{2}-\d{2}-(\d{2}|[A-Z]{2}))|(\d{2}-(\d{2}-[A-Z]{2}|[A-Z]{2}-\d{2})))$");
            return regex.IsMatch(licensePlate);
        }

        private static bool IsValidBrand(string brand)
        {
            return listBrands.Contains(brand);
        }

    }
}
