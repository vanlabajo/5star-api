using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    class MonthlySalesEntityConfiguration : IEntityTypeConfiguration<MonthlySales>
    {
        public void Configure(EntityTypeBuilder<MonthlySales> builder)
        {
            builder.ToTable("MonthlySales");
            builder.HasKey(o => o.Year);
            builder.Property(o => o.Year).ValueGeneratedNever();
        }
    }
}
