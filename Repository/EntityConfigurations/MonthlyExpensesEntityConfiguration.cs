using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    class MonthlyExpensesEntityConfiguration : IEntityTypeConfiguration<MonthlyExpenses>
    {
        public void Configure(EntityTypeBuilder<MonthlyExpenses> builder)
        {
            builder.ToTable("MonthlyExpenses");
            builder.HasKey(o => o.Year);
            builder.Property(o => o.Year).ValueGeneratedNever();
        }
    }
}
