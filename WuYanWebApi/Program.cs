
using Microsoft.EntityFrameworkCore;
using WuYanWebApi.Models;
using WuYanWebApi.Controllers;
using WuYanWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace WuYanWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContextPool<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.


            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(
               key: Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
           ValidateIssuer = false,
           ValidateAudience = false,
           ValidateLifetime = true,
           ClockSkew = TimeSpan.Zero
       };
   });
            var app = builder.Build();
            
           

            // 添加授权服务
            builder.Services.AddAuthorization();

            // 注册自定义服务
           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
        }
    }



