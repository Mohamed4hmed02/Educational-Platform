using Educational_Platform.Domain.Entities;
using Educational_Platform.Infrastructure.EntitiesSpecification;
using Microsoft.EntityFrameworkCore;

namespace Educational_Platform.Infrastructure.Implementations.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdminEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CartDetailEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CartEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourseUserTakeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TopicEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UnitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserCourseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
