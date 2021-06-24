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

            builder.Property(o => o.Jan).HasPrecision(18, 6);
            builder.Property(o => o.Feb).HasPrecision(18, 6);
            builder.Property(o => o.Mar).HasPrecision(18, 6);
            builder.Property(o => o.Apr).HasPrecision(18, 6);
            builder.Property(o => o.May).HasPrecision(18, 6);
            builder.Property(o => o.Jun).HasPrecision(18, 6);
            builder.Property(o => o.Jul).HasPrecision(18, 6);
            builder.Property(o => o.Aug).HasPrecision(18, 6);
            builder.Property(o => o.Sep).HasPrecision(18, 6);
            builder.Property(o => o.Oct).HasPrecision(18, 6);
            builder.Property(o => o.Nov).HasPrecision(18, 6);
            builder.Property(o => o.Dec).HasPrecision(18, 6);
        }
    }
}
