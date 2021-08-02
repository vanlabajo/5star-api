using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Repository
{
    public class MariaDbFiveStarDbContext : FiveStarDbContext
    {
        public MariaDbFiveStarDbContext(DbContextOptions<MariaDbFiveStarDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseMySql(new MySqlConnection(), MariaDbServerVersion.LatestSupportedServerVersion);
            }
        }
    }
}
