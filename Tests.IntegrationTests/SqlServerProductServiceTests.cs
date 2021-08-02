using Xunit;

namespace Tests.IntegrationTests
{
    public class SqlServerProductServiceTests : ProductServiceTests, IClassFixture<SharedSqlServerDatabaseFixture>
    {
        public SqlServerProductServiceTests(SharedSqlServerDatabaseFixture sqlDatabaseFixture)
            : base(sqlDatabaseFixture)
        {
        }
    }
}
