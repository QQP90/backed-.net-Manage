using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entities;

namespace WebApplication1.Infrastructure
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 为每个实体类添加一个 DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        // 添加其他实体...
        public DbSet<Order> Orders { get; set; }
        /// <summary>
        /// 配置数据库模型
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 User 表
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
            // 配置 Order 表
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                // 数据库层默认值：当 INSERT 没有显式传 orderDate 时，由数据库生成当前时间
                // 如果不配：当应用端忘记赋值时，SQL Server 可能落入默认值（例如 1900-01-01 或 0001-01-01）
                entity.Property(e => e.orderDate).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.HasIndex(e => e.orderDate);
            });
        }
    }
}
