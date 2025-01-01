using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Domain.EntitiesSpecification
{
    public class OrderDetailEntityTypeConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");

            builder.Property(p => p.Price)
                .HasConversion<decimal>();

            builder
                .HasKey(k => new { k.OrderId, k.ProductId, k.ProductType });

            builder
                .HasOne(o => o.Order)
                .WithMany(m => m.Details)
                .HasForeignKey(f => f.OrderId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
