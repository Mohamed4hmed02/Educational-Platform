using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Domain.EntitiesSpecification
{
	public class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
	{
		public void Configure(EntityTypeBuilder<Cart> builder)
		{
            builder.ToTable("Carts");

            builder
				.HasOne(o => o.User)
				.WithMany(m => m.Carts)
				.HasForeignKey(f => f.UserId)
				.HasPrincipalKey(p => p.Id)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Property(p => p.Total)
				.HasConversion<decimal>();
		}
	}
}
