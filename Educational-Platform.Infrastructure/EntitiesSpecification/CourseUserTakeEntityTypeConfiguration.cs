using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Infrastructure.EntitiesSpecification
{
    public class CourseUserTakeEntityTypeConfiguration : IEntityTypeConfiguration<OldUserCourse>
    {
        public void Configure(EntityTypeBuilder<OldUserCourse> builder)
        {
            builder.ToTable("OldUserCourses");

            builder
                .HasOne(c => c.User)
                .WithMany(u => u.CoursesTakes)
                .HasForeignKey(c => c.UserId)
                .HasPrincipalKey(u => u.Id);
        }
    }
}
