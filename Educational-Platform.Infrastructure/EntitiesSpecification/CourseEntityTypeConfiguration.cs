using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Infrastructure.EntitiesSpecification
{
    public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");

            builder
                .HasMany(m => m.Units)
                .WithOne(o => o.Course)
                .HasForeignKey(f => f.CourseId)
                .HasPrincipalKey(p => p.Id);

            builder.Property(p => p.Price)
                .HasConversion<decimal>();

            builder.HasIndex(c => c.Name).IsUnique();

            string query =
                """
                CAST (CASE
                WHEN CAST(GETUTCDATE() AS DATE) <= DiscountEndTime 
                THEN ROUND(Price - (Discount / 100.0 * Price), 2)
                ELSE Price
                END AS DECIMAL)
                """;

            builder.Property(c => c.NetPrice)
                .HasComputedColumnSql(query);

            builder.Property(c => c.NetPrice)
                .HasConversion<decimal>();
        }
    }
}
