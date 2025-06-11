using Microsoft.AspNetCore.Mvc;
using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Services;
using ParkingSystem.Application.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ParkingSystem.Application.Exceptions;
using ParkingSystem.Core.Constants;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            try
            {
                var result = await _userService.RegisterUser(userDto);
                if (result != null)
                {
                    _logger.LogInformation("User " + result.username + " created successfully.");
                    return Ok("User registered successfully.");
                }
                _logger.LogError("Failed to create user.");
                return BadRequest("Failed to create user.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating user: " + ex.Message);
                return BadRequest("Failed to create user.");
            }

        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var token = _userService.Authenticate(email, password);
            return Ok(new { Token = token });
        }

        [HttpPut("update")]
        [Authorize(Policy = "AllRolesExceptUnauthenticated")]
        public async Task<IActionResult> Update(UserDTO userDto)
        {
            try
            {
                var result = await _userService.UpdateUser(userDto);
                if (result != null)
                {
                    return Ok("User updated successfully.");
                }
                return BadRequest("Failed to update user.");
            }
            catch (UserNotFoundException)
            {
                return NotFound("The provided user was not found");
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal error occurred when updating the user");
            }

        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AllRolesExceptUnauthenticated")]
        public IActionResult Delete(long id)
        {
            var result = _userService.DeleteUser(id);
            if (result != null)
            {
                return Ok("User deleted successfully.");
            }
            return NotFound("User not found.");
        }

        [HttpPost("registerParkManager")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> RegisterParkManager(UserDTO userDto)
        {
            try
            {
                userDto.role = RoleNames.ParkManager;
                var result = await _userService.RegisterParkManager(userDto);
                if (result != null)
                {
                    _logger.LogInformation("Park Manager " + result.username + " created successfully.");
                    return Ok("Park Manager registered successfully.");
                }
                _logger.LogError("Failed to create Park Manager.");
                return BadRequest("Failed to create Park Manager.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating Park Manager: " + ex.Message);
                return BadRequest("Failed to create Park Manager.");
            }

        }

        [HttpPost("activateSubscription")]
        [Authorize(Policy = "AllRolesExceptUnauthenticated")]
        public async Task<IActionResult> ActivateSubscription(long userId, long parkId)
        {
            try
            {
                var result = await _userService.ActivateSubscription(userId, parkId);
                if (result)
                {
                    _logger.LogInformation($"User {userId} activated monthly subscription for parking {parkId}.");
                    return Ok("Subscription activated sucessfully.");
                }
                _logger.LogError("Failed to activating subscription.");
                return BadRequest("Failed to activating subscription.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error activating subscription: " + ex.Message);
                return BadRequest("Failed to activating subscription.");
            }

        }
    }
}
