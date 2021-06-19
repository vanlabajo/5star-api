using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    class InvoiceEntityConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.ReferenceNumber).IsRequired();
            builder.Property(o => o.CreatedTime);

            builder.OwnsMany(o => o.Items, cfg =>
            {
                cfg.WithOwner().HasForeignKey("InvoiceId");
                cfg.Property<int>("Id");
                cfg.HasKey("Id");
                cfg.Property("Id").ValueGeneratedOnAdd();

                cfg.HasOne(x => x.Product).WithMany();
                cfg.Property(x => x.Quantity);
            });

            builder.HasIndex(o => o.ReferenceNumber);
        }
    }
}
