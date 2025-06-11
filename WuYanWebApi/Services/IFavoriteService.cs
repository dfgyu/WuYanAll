
using Microsoft.EntityFrameworkCore;
using WuYanWebApi.Models;

namespace WuYanWebApi.Services
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavorite(int userId, string contentType, int contentId);
        Task<bool> IsFavorite(int userId, string contentType, int contentId);
        Task<IEnumerable<Favorite>> GetUserFavorites(int userId);
        Task<int> GetFavoriteCount(string contentType, int contentId);
    }

    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;

        public FavoriteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleFavorite(int userId, string contentType, int contentId)
        {
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&
                    f.ContentType == contentType &&
                    f.ContentId == contentId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // 已取消收藏
            }

            var favorite = new Favorite
            {
                UserId = userId,
                ContentType = contentType,
                ContentId = contentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return true; // 已收藏
        }

        public async Task<bool> IsFavorite(int userId, string contentType, int contentId)
        {
            return await _context.Favorites
                .AnyAsync(f =>
                    f.UserId == userId &&
                    f.ContentType == contentType &&
                    f.ContentId == contentId);
        }

        public async Task<IEnumerable<Favorite>> GetUserFavorites(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetFavoriteCount(string contentType, int contentId)
        {
            return await _context.Favorites
                .CountAsync(f =>
                    f.ContentType == contentType &&
                    f.ContentId == contentId);
        }
    }
}
