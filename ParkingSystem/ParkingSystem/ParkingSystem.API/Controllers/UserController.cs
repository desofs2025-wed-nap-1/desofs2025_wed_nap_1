using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;
using System.Threading.Tasks;

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
                    // result = (UserDTO) result.;
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
        public IActionResult Update(UserDTO userDto)
        {
            var result = _userService.UpdateUser(userDto);
            if (result != null)
            {
                return Ok("User updated successfully.");
            }
            return BadRequest("Failed to update user.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(long id)
        {
            var result = _userService.DeleteUser(id);
            if (result != null)
            {
                return Ok("User deleted successfully.");
            }
            return NotFound("User not found.");
        }
    }
}
