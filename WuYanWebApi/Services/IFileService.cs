namespace WuYanWebApi.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subDirectory);
        Task<string> SaveImageAsync(IFormFile imageFile, string subDirectory, int maxWidth = 800);
        bool DeleteFile(string filePath);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("无文件");

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", subDirectory);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subDirectory}/{uniqueFileName}";
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string subDirectory, int maxWidth = 800)
        {
            // 简化实现，实际项目中应使用ImageSharp等库处理图片
            return await SaveFileAsync(imageFile, Path.Combine("images", subDirectory));
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
                return false;

            try
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除图片错误: {Path}", fullPath);
                return false;
            }
        }
    }
}
