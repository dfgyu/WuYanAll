namespace WuYanWebApi.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContentType { get; set; } // "Article", "Image", "Question"
        public int ContentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
