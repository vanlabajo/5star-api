using Core;
using Microsoft.EntityFrameworkCore;
using Repository.EntityConfigurations;

namespace Repository
{
    public class FiveStarDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<MonthlySales> MonthlySales { get; set; }
        public DbSet<MonthlyExpenses> MonthlyExpenses { get; set; }

        public FiveStarDbContext() { }
        public FiveStarDbContext(DbContextOptions<FiveStarDbContext> options) : base(options) {}
        public FiveStarDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MonthlySalesEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MonthlyExpensesEntityConfiguration());
        }
    }
}
