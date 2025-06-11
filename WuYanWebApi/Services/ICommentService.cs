
using Microsoft.EntityFrameworkCore;
using WuYanWebApi.Models;

namespace WuYanWebApi.Services
{
    public interface ICommentService
    {
        Task<Comment> AddComment(int userId, string contentType, int contentId, string text);
        Task<Comment> UpdateComment(int commentId, int userId, string text);
        Task<bool> DeleteComment(int commentId, int userId);
        Task<IEnumerable<Comment>> GetComments(string contentType, int contentId);
        Task<IEnumerable<Comment>> GetUserComments(int userId);
        Task<Comment?> GetCommentById(int commentId);
    }
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddComment(int userId, string contentType, int contentId, string text)
        {
            var comment = new Comment
            {
                UserId = userId,
                ContentType = contentType,
                ContentId = contentId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateComment(int commentId, int userId, string text)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
                throw new Exception("Comment not found or not owned by user");

            comment.Text = text;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteComment(int commentId, int userId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Comment>> GetComments(string contentType, int contentId)
        {
            return await _context.Comments
                .Where(c => c.ContentType == contentType && c.ContentId == contentId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetUserComments(int userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentById(int commentId)
        {
            return await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }
    }
}
