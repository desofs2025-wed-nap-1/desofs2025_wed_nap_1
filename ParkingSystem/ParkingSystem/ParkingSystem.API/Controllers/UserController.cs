/*namespace ParkingSystem.API.Controllers
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
        public IActionResult Register(UserDTO userDto)
        {
            _userService.RegisterUser(userDto);
            return Ok("User registered successfully.");
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
            _userService.UpdateUser(userDto);
            return Ok("User updated successfully.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            _userService.DeleteUser(id);
            return Ok("User deleted.");
        }
    }
}*/
