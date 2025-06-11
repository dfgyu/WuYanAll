using Microsoft.EntityFrameworkCore;
using WuYanWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using Image = WuYanWebApi.Models.Image;


namespace WuYanWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyContentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DailyContentController(AppDbContext context)
        {
            _context = context;
        }

        // 获取今日推荐内容
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayContent()
        {
            var today = DateTime.Today;

            var article = await _context.Articles
                .Where(a => a.PublishDate.Date == today)
                .FirstOrDefaultAsync();

            var image = await _context.Images
                .Where(i => i.Date.Date == today)
                .FirstOrDefaultAsync();

            var question = await _context.Questions
                .Where(q => q.Date.Date == today)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                Article = article,
                Image = image,
                Question = question
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