using Core;
using Microsoft.EntityFrameworkCore;
using Repository.EntityConfigurations;

namespace Repository
{
    public class FiveStarDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public FiveStarDbContext(DbContextOptions<FiveStarDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceEntityConfiguration());
        }
    }
}
