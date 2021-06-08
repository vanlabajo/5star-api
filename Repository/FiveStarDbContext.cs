using Core;
using Microsoft.EntityFrameworkCore;
using Repository.EntityConfigurations;

namespace Repository
{
    public class FiveStarDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public FiveStarDbContext(DbContextOptions<FiveStarDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        }
    }
}
