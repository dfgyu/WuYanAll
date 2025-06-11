using WuYanWebApi.Services;
using WuYanWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace WuYanWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            try
            {
                var user = await _authService.GetUserById(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserResponse model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            // 在实际应用中，这里应该有更新用户信息的逻辑
            // 为简化示例，我们直接返回当前用户信息
            try
            {
                var user = await _authService.GetUserById(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
