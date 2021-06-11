using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.Name).IsRequired();
            builder.Property(o => o.Upc).IsRequired();
            builder.Property(o => o.Cost).HasPrecision(18, 6);
            builder.Property(o => o.Price).HasPrecision(18, 6);
            builder.Property(o => o.Quantity);
            builder.Property(o => o.LastUpdateTimeUtc);
            builder.Property(o => o.LastUpdatedBy);
            builder.Property(o => o.CreatedTimeUtc);
            builder.Property(o => o.CreatedBy);
            builder.Property(o => o.Timestamp).IsRowVersion();

            builder.HasIndex(o => o.Name);
            builder.HasIndex(o => o.Upc);
            builder.HasIndex(o => new { o.Name, o.Upc });
        }
    }
}
