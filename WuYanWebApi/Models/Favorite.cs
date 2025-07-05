namespace WuYanWebApi.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContentType { get; set; } // "Article", "Image", "Question"
        public int ContentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!; // 导航属性，指向用户实体
        // 确保ContentType和ContentId的组合是唯一的
        public override bool Equals(object? obj)
        {
            if (obj is not Favorite other)
                return false;
            return UserId == other.UserId &&
                   ContentType == other.ContentType &&
                   ContentId == other.ContentId;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, ContentType, ContentId);
        }

    }
}
