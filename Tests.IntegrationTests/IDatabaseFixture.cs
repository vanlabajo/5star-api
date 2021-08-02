using Repository;
using System.Data.Common;

namespace Tests.IntegrationTests
{
    public interface IDatabaseFixture
    {
        DbConnection DbConnection { get; }
        FiveStarDbContext CreateDbContext(DbTransaction transaction = null);
    }
}
