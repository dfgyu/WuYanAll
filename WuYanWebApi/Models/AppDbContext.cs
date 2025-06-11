using Microsoft.EntityFrameworkCore;



namespace WuYanWebApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置Favorite实体
            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.ContentType, f.ContentId })
                .IsUnique();

            // 配置Comment实体
            modelBuilder.Entity<Comment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}