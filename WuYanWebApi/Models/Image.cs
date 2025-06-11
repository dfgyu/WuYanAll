namespace WuYanWebApi.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public DateTime Date { get; set; }
    }
}
