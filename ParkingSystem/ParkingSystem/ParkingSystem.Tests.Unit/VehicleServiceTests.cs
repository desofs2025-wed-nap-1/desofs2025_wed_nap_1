using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.Services;
using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Mappers;

namespace ParkingSystem.Tests.Unit
{
    public class VehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object);
        }

        [Fact]
        public async Task AddVehicleToUser_ShouldReturnVehicleDTO_WhenValidData()
        {
            var vehicleDto = new VehicleDTO
            {
                licensePlate = "AB-12-CD",
                brand = "Toyota",
                username = "user123"
            };

            var vehicleDomain = VehicleMapper.ToVehicleDomain(vehicleDto);

            _vehicleRepositoryMock
                .Setup(repo => repo.AddVehicle(It.IsAny<Vehicle>(), vehicleDto.username))
                .ReturnsAsync(vehicleDomain);

            var result = await _vehicleService.AddVehicleToUser(vehicleDto);

            Assert.NotNull(result);
            Assert.Equal(vehicleDto.licensePlate, result.licensePlate);
        }

        [Fact]
        public async Task AddVehicleToUser_ShouldThrow_WhenLicensePlateIsInvalid()
        {
            var vehicleDto = new VehicleDTO
            {
                licensePlate = "1234-XYZ",
                brand = "Toyota",
                username = "user123"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _vehicleService.AddVehicleToUser(vehicleDto));
        }

        [Fact]
        public async Task AddVehicleToUser_ShouldThrow_WhenBrandIsInvalid()
        {
            var vehicleDto = new VehicleDTO
            {
                licensePlate = "AB-12-CD",
                brand = "InvalidBrand",
                username = "user123"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _vehicleService.AddVehicleToUser(vehicleDto));
        }
    }
}
