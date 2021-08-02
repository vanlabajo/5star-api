using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class SqlServerFiveStarDbContext : FiveStarDbContext
    {
        public SqlServerFiveStarDbContext(DbContextOptions<SqlServerFiveStarDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer();
            }
        }
    }
}
