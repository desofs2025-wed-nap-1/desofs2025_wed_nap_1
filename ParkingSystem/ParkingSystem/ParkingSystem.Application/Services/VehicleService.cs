using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Interfaces;
using  ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Mappers;
using System.Text.RegularExpressions;
using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
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

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<VehicleDTO?> AddVehicleToUser(VehicleDTO vehicleDto)
        {
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
            return result != null ? VehicleMapper.ToVehicleDto(result) : null;
        }

        public async Task<VehicleDTO?> UpdateVehicle(VehicleDTO vehicleDto, long vehicleId)
        {
            if (!IsValidLicensePlate(vehicleDto.licensePlate))
            {
                throw new ArgumentException("License plate format is invalid.");
            }
             if (!IsValidBrand(vehicleDto.brand))
            {
                throw new ArgumentException("Brand is invalid.");
            }
            var vehicle = VehicleMapper.ToVehicleDomain(vehicleDto);
            var updatedVehicle = await _vehicleRepository.UpdateVehicle(vehicle, vehicleId);            
            return updatedVehicle != null ? VehicleMapper.ToVehicleDto(updatedVehicle) : null;
        }

        public async Task<VehicleDTO?> DeleteVehicle(long id)
        {
            var deletedVehicle = await _vehicleRepository.DeleteVehicle(id);            
            return deletedVehicle != null ? VehicleMapper.ToVehicleDto(deletedVehicle) : null;
        }
        
        public async Task<IEnumerable<VehicleDTO>> GetVehiclesByUser(long userId)
        {
            var result = await _vehicleRepository.GetAllVehiclesFromUser(userId);
            List<VehicleDTO> listVehicles = new List<VehicleDTO>();
            foreach (var item in result)
            {
                listVehicles.Add(VehicleMapper.ToVehicleDto(item));
            }
            return listVehicles;
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
