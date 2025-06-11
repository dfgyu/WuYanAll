namespace WuYanWebApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContentType { get; set; } // "Article", "Image", "Question"
        public int ContentId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
