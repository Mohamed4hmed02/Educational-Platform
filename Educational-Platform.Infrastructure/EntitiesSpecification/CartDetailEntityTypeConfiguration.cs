using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Infrastructure.EntitiesSpecification
{
    public class CartDetailEntityTypeConfiguration : IEntityTypeConfiguration<CartDetail>
    {
        public void Configure(EntityTypeBuilder<CartDetail> builder)
        {
            builder.ToTable("CartDetails");

            builder.HasKey(k => new { k.ProductId, k.CartId, k.ProductType });

            builder.HasOne(o => o.Cart)
                .WithMany(m => m.CartDetails)
                .HasForeignKey(f => f.CartId)
                .HasPrincipalKey(p => p.Id);
        }
    }
}
