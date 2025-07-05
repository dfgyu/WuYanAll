using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WuYanWebApi.Models;
using WuYanWebApi.Services;
using Image = WuYanWebApi.Models.Image;


namespace WuYanWebApi.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class DailyContentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFavoriteService _favoriteService;
        private readonly ICommentService _commentService;


        public DailyContentController(AppDbContext context)
        {
            _context = context;
        }

        // 获取今日推荐内容
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayContent()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var article = await _context.Articles
                .Where(a => a.PublishDate >= today && a.PublishDate < tomorrow)
                .FirstOrDefaultAsync();

            var image = await _context.Images
                .Where(i => i.Date >= today && i.Date < tomorrow)
                .FirstOrDefaultAsync();

            var question = await _context.Questions
                .Where(q => q.Date >= today && q.Date < tomorrow)
                .FirstOrDefaultAsync();

            // 获取收藏和评论信息
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            return Ok(new
            {
                Article = article != null ? new
                {
                    article.Id,
                    article.Title,
                    article.Auther,
                    article.Content,
                    article.PublishDate,
                    FavoriteCount = await _favoriteService.GetFavoriteCount("Article", article.Id),
                    CommentCount = await _commentService.GetCommentCount("Article", article.Id),
                    IsFavorite = userId.HasValue ?
                        await _favoriteService.IsFavorite(userId.Value, "Article", article.Id) : false
                } : null,

                Image = image != null ? new
                {
                    image.Id,
                    image.Title,
                    image.Description,
                    image.Url,
                    image.Date,
                    FavoriteCount = await _favoriteService.GetFavoriteCount("Image", image.Id),
                    CommentCount = await _commentService.GetCommentCount("Image", image.Id),
                    IsFavorite = userId.HasValue ?
                        await _favoriteService.IsFavorite(userId.Value, "Image", image.Id) : false
                } : null,

                Question = question != null ? new
                {
                    question.Id,
                    question.QuestionText,
                    question.Answer,
                    question.Date,
                    FavoriteCount = await _favoriteService.GetFavoriteCount("Question", question.Id),
                    CommentCount = await _commentService.GetCommentCount("Question", question.Id),
                    IsFavorite = userId.HasValue ?
                        await _favoriteService.IsFavorite(userId.Value, "Question", question.Id) : false
                } : null
            });
        }
        // 添加文章 - 修正版
        [Authorize]
        [HttpPost("article")]
        public async Task<IActionResult> AddArticle([FromBody] Article article)
        {
            // 获取当前用户ID
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            article.PublishDate = DateTime.Now;
            // 可以添加用户ID到文章，如果需要在内容中记录作者
            //article.AuthorId = userId;

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetArticle),
                new { id = article.Id },
                article
            );
        }

        // 获取单个文章
        [HttpGet("article/{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        // 添加图片
        [Authorize]
        [HttpPost("image")]
        public async Task<IActionResult> AddImage([FromBody] Image image)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            image.Date = DateTime.Now;
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetArticle),
                new { id = image.Id },
                image
);
        }

        // 获取单个图片
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        // 添加问答
        [Authorize]
        [HttpPost("question")]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            question.Date = DateTime.Now;
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
               nameof(GetArticle),
               new { id = question.Id },
               question
);
        }

        // 获取单个问答
        [HttpGet("question/{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }
    }
}