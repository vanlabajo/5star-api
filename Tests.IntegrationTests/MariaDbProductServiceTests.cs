using Xunit;

namespace Tests.IntegrationTests
{
    public class MariaDbProductServiceTests : ProductServiceTests, IClassFixture<SharedMariaDbDatabaseFixture>
    {
        public MariaDbProductServiceTests(SharedMariaDbDatabaseFixture mariaDatabaseFixture)
            : base(mariaDatabaseFixture)
        {
        }
    }
}
