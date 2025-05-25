using Microsoft.AspNetCore.Mvc;
using  ParkingSystem.Application.DTOs;
using  ParkingSystem.Application.Services;
using  ParkingSystem.Application.Interfaces;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            var result = await _userService.RegisterUser(userDto);
            if (result != null)
            {
                return Ok("User registered successfully.");
            }
            return BadRequest("Failed to create user.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var token = await _userService.Authenticate(email, password);
            return Ok(new { Token = token });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UserDTO userDto, long userId)
        {
            var result = await _userService.UpdateUser(userDto, userId);
            if (result != null)
            {
                return Ok("User updated successfully.");
            }
            return BadRequest("Failed to update user.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _userService.DeleteUser(id);
            if (result != null)
            {
                return Ok("User deleted successfully.");
            }
            return NotFound("User not found.");
        }
    }
}
