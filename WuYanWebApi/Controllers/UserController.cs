using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WuYanWebApi.Models;
using WuYanWebApi.Services;


namespace WuYanWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<UserController> _logger;

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
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfileUpdate model)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                // 更新用户信息
                if (!string.IsNullOrWhiteSpace(model.Bio))
                    user.Bio = model.Bio;

                // 处理头像上传
                if (model.Avatar != null && model.Avatar.Length > 0)
                {
                    // 删除旧头像（如果有）
                    if (!string.IsNullOrEmpty(user.AvatarUrl))
                        _fileService.DeleteFile(user.AvatarUrl);

                    // 保存新头像
                    user.AvatarUrl = await _fileService.SaveImageAsync(
                        model.Avatar, "avatars");
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Bio = user.Bio,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                if (avatar == null || avatar.Length == 0)
                    return BadRequest("无文件");

                // 删除旧头像（如果有）
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                    _fileService.DeleteFile(user.AvatarUrl);

                // 保存新头像
                user.AvatarUrl = await _fileService.SaveImageAsync(avatar, "avatars");
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { avatarUrl = user.AvatarUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新头像失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
