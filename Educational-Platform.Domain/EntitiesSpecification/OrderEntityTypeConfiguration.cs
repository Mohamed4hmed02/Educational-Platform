using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Domain.EntitiesSpecification
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder
                .HasOne(o => o.User)
                .WithMany(o => o.Orders)
                .HasForeignKey(f => f.UserId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.Total)
                .HasConversion<decimal>();
        }
    }
}
