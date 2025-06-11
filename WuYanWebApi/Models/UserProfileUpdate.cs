namespace WuYanWebApi.Models
{
    public class UserProfileUpdate
    {
        public string? Bio { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
