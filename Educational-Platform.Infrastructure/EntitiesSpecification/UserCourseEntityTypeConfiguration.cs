using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Infrastructure.EntitiesSpecification
{
    public class UserCourseEntityTypeConfiguration : IEntityTypeConfiguration<UserCourse>
    {
        public void Configure(EntityTypeBuilder<UserCourse> builder)
        {
            builder.ToTable("UsersCourses");

            builder.HasKey(k => new { k.UserId, k.CourseId });
        }
    }
}
