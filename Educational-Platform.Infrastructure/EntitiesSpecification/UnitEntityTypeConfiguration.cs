using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Infrastructure.EntitiesSpecification
{
    public class UnitEntityTypeConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Units");

            builder
                .HasIndex(i => new { i.Name, i.CourseId }).IsUnique();
        }
    }
}
