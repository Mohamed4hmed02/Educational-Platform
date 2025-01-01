using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educational_Platform.Domain.EntitiesSpecification
{
    public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder
                .HasKey(x => x.Id);

            builder
                .HasOne(o => o.Order)
                .WithMany(m => m.Payments)
                .HasForeignKey(f => f.OrderId)
                .HasPrincipalKey(p => p.Id);

            builder.Property(p => p.Total)
                .HasConversion<decimal>();
        }
    }
}
