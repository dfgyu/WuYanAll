using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WuYanWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace WuYanWebApi.Services
{
    public interface IAuthService {
        Task<UserResponse> Register(RegisterRequest model);
        Task<string> Login(LoginRequest model);
        Task<UserResponse> GetUserById(int id);
        Task<bool> CheckUsernameExists(string username);
        Task<bool> CheckEmailExists(string email);

    }
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserResponse> Register(RegisterRequest model)
        {
            // 检查用户名和邮箱是否已存在
            if (await CheckUsernameExists(model.Username))
                throw new Exception("Username already exists");

            if (await CheckEmailExists(model.Email))
                throw new Exception("Email already exists");

            // 创建用户
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapUserToResponse(user);
        }

        public async Task<string> Login(LoginRequest model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail);

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            return GenerateJwtToken(user);
        }

        public async Task<UserResponse> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return MapUserToResponse(user);
        }

        public async Task<bool> CheckUsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> CheckEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + _configuration["Jwt:Secret"]);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private UserResponse MapUserToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

    }
}
