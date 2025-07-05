using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WuYanWebApi.Services;

namespace WuYanWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(
            IFavoriteService favoriteService,
            ILogger<FavoriteController> logger)
        {
            _favoriteService = favoriteService;
            _logger = logger;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _favoriteService.ToggleFavorite(
                    userId, request.ContentType, request.ContentId);

                return Ok(new
                {
                    isFavorite = result,
                    count = await _favoriteService.GetFavoriteCount(
                        request.ContentType, request.ContentId)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("status/{contentType}/{contentId}")]
        public async Task<IActionResult> GetFavoriteStatus(string contentType, int contentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var isFavorite = await _favoriteService.IsFavorite(
                    userId, contentType, contentId);
                var count = await _favoriteService.GetFavoriteCount(contentType, contentId);

                return Ok(new { isFavorite, count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting favorite status");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyFavorites()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var favorites = await _favoriteService.GetUserFavorites(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user favorites");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }

    public class FavoriteRequest
    {
        public string ContentType { get; set; } = string.Empty;
        public int ContentId { get; set; }
    }
}
