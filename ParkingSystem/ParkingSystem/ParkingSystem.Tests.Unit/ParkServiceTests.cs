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
    public class ParkServiceTests
    {
        private readonly Mock<IParkRepository> _parkRepositoryMock;
        private readonly ParkService _parkService;

        public ParkServiceTests()
        {
            _parkRepositoryMock = new Mock<IParkRepository>();
            _parkService = new ParkService(_parkRepositoryMock.Object);
        }

        [Fact]
        public async Task AddPark_ShouldReturnParkDTO_WhenValidData()
        {
            var parkDto = new ParkDTO
            {
                name = "Central Park",
                location = "Downtown",
                capacity = 100,
                gateOpen = true
            };

            var parkDomain = ParkMapper.ToParkDomain(parkDto);

            _parkRepositoryMock
                .Setup(repo => repo.AddPark(It.IsAny<Park>()))
                .ReturnsAsync(parkDomain);

            var result = await _parkService.AddPark(parkDto);

            Assert.NotNull(result);
            Assert.Equal(parkDto.name, result?.name);
            Assert.Equal(parkDto.location, result?.location);
            Assert.Equal(parkDto.capacity, result?.capacity);
            Assert.Equal(parkDto.gateOpen, result?.gateOpen);
        }

        [Fact]
        public async Task UpdatePark_ShouldReturnUpdatedParkDTO_WhenParkExists()
        {
            var parkDto = new ParkDTO
            {
                name = "Updated Park",
                location = "Uptown",
                capacity = 200,
                gateOpen = false
            };

            var parkDomain = ParkMapper.ToParkDomain(parkDto);

            _parkRepositoryMock
                .Setup(repo => repo.UpdatePark(It.IsAny<Park>()))
                .ReturnsAsync(parkDomain);

            var result = await _parkService.UpdatePark(parkDto);

            Assert.NotNull(result);
            Assert.Equal(parkDto.name, result?.name);
            Assert.Equal(parkDto.location, result?.location);
            Assert.Equal(parkDto.capacity, result?.capacity);
            Assert.Equal(parkDto.gateOpen, result?.gateOpen);
        }

        [Fact]
        public async Task DeletePark_ShouldReturnDeletedParkDTO_WhenParkExists()
        {
            long parkId = 1;

            var parkDomain = new Park
            {
                name = "Old Park",
                location = "Old Location",
                capacity = 50,
                gateOpen = true
            };

            _parkRepositoryMock
                .Setup(repo => repo.DeletePark(parkId))
                .ReturnsAsync(parkDomain);

            var result = await _parkService.DeletePark(parkId);

            Assert.NotNull(result);
            Assert.Equal(parkDomain.name, result?.name);
            Assert.Equal(parkDomain.location, result?.location);
            Assert.Equal(parkDomain.capacity, result?.capacity);
            Assert.Equal(parkDomain.gateOpen, result?.gateOpen);
        }

        [Fact]
        public async Task GetAvailableParks_ShouldReturnListOfParkDTOs()
        {
            var parksDomain = new List<Park>
            {
                new Park { name = "Park1", location = "Loc1", capacity = 100, gateOpen = true },
                new Park { name = "Park2", location = "Loc2", capacity = 150, gateOpen = false }
            };

            _parkRepositoryMock
                .Setup(repo => repo.GetAvailableParks())
                .ReturnsAsync(parksDomain);

            var result = await _parkService.GetAvailableParks();

            Assert.NotNull(result);
            Assert.Collection(result,
                park =>
                {
                    Assert.Equal("Park1", park.name);
                    Assert.Equal("Loc1", park.location);
                    Assert.Equal(100, park.capacity);
                    Assert.True(park.gateOpen);
                },
                park =>
                {
                    Assert.Equal("Park2", park.name);
                    Assert.Equal("Loc2", park.location);
                    Assert.Equal(150, park.capacity);
                    Assert.False(park.gateOpen);
                });
        }
    }
}
