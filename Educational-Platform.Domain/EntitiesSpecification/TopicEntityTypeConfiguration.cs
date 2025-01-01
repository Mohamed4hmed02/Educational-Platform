using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Domain.EntitiesSpecification
{
	public class TopicEntityTypeConfiguration : IEntityTypeConfiguration<Topic>
	{
		public void Configure(EntityTypeBuilder<Topic> builder)
		{
            builder.ToTable("Topics");

            builder
				.HasOne(o => o.Unit)
				.WithMany(m => m.Topics)
				.HasForeignKey(f => f.UnitId)
				.HasPrincipalKey(p => p.Id);
		}
	}
}
