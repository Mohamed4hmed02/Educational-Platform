using Educational_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Educational_Platform.Domain.EntitiesSpecification
{
    public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.Property(p => p.Price)
                .HasConversion<decimal>();
        }
    }
}
