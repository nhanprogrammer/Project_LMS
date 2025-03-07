using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtReponsitory _jwtReponsitory;

        public AuthController(IJwtReponsitory jwtReponsitory)
        {
            _jwtReponsitory = jwtReponsitory;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _jwtReponsitory.AuthenticateAsync(loginModel.Username, loginModel.Password);
            if (account == null)
            {
                return Unauthorized();
            }

            var token = _jwtReponsitory.GenerateToken(account);
            return Ok(token);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (registerModel == null)
                return BadRequest("Invalid input.");

            var newUser = await _jwtReponsitory.RegisterAsync(registerModel.Username, registerModel.Password/*, registerModel.Email*/);

            if (newUser == null)
                return Conflict("Username already exists.");

            return Ok(newUser);
        }
    }
}
