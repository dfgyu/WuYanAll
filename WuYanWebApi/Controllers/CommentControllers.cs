using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WuYanWebApi.Services;




namespace WuYanWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            ICommentService commentService,
            ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet("{contentType}/{contentId}")]
        public async Task<IActionResult> GetComments(string contentType, int contentId)
        {
            try
            {
                var comments = await _commentService.GetComments(contentType, contentId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获得评论失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var comment = await _commentService.AddComment(
                    userId, request.ContentType, request.ContentId, request.Text);

                return CreatedAtAction(
                    nameof(GetComment),
                    new { id = comment.Id },
                    comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "评论失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var comment = await _commentService.UpdateComment(id, userId, request.Text);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新评论失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _commentService.DeleteComment(id, userId);

                if (result)
                    return NoContent();

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除评论失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            try
            {
                var comment = await _commentService.GetCommentById(id);
                if (comment == null) return NotFound();
                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获得评论失败");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }

    public class CommentRequest
    {
        public string ContentType { get; set; } = string.Empty;
        public int ContentId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class CommentUpdateRequest
    {
        public string Text { get; set; } = string.Empty;
    }
}
