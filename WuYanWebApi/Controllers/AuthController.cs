using Microsoft.AspNetCore.Mvc;
using WuYanWebApi.Models;
using WuYanWebApi.Services;


namespace WuYanWebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                var user = await _authService.Register(model);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                var token = await _authService.Login(model);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("check-username")]
        public async Task<IActionResult> CheckUsernameExists([FromQuery] string username)
        {
            var exists = await _authService.CheckUsernameExists(username);
            return Ok(new { exists });
        }

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
        {
            var exists = await _authService.CheckEmailExists(email);
            return Ok(new { exists });
        }



    }
}

