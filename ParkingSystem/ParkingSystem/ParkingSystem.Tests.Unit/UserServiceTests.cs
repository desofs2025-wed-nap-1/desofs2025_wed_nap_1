using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using ParkingSystem.Application.Services;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Mappers;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrow_WhenUsernameTaken()
    {
        var userDto = new UserDTO { username = "takenuser" };

        _userRepoMock.Setup(repo => repo.IsUsernameTaken("takenuser")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ArgumentException>(() => _userService.RegisterUser(userDto));
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnUserDTO_WhenSuccess()
    {
        var userDto = new UserDTO { username = "newuser" };
        var userDomain = UserMapper.ToUserDomain(userDto);

        _userRepoMock.Setup(repo => repo.IsUsernameTaken("newuser")).ReturnsAsync(false);
        _userRepoMock.Setup(repo => repo.AddUser(It.IsAny<ParkingSystem.Core.Aggregates.User>()))
            .ReturnsAsync(userDomain);

        var result = await _userService.RegisterUser(userDto);

        Assert.NotNull(result);
        Assert.Equal("newuser", result.username);
    }
}
