using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class SalesServiceTests : IDisposable
    {
        private readonly SqliteConnection sqliteConnection;
        private readonly DbContextOptions<FiveStarDbContext> dbContextOptions;

        public SalesServiceTests()
        {
            sqliteConnection = new SqliteConnection("Filename=:memory:");
            sqliteConnection.Open();

            dbContextOptions = new DbContextOptionsBuilder<FiveStarDbContext>()
                .UseSqlite(sqliteConnection)
                .Options;

            using var dbContext = new FiveStarDbContext(dbContextOptions);
            dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            sqliteConnection.Dispose();
        }

        [Fact]
        public async Task ShouldReturnMonthlySales()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var year2021 = new MonthlySales(2021);
            year2021.Jun = 1000m;

            var year2022 = new MonthlySales(2022);
            year2022.Jun = 2000m;

            dbContext.MonthlySales.AddRange(year2021, year2022);
            dbContext.SaveChanges();

            var service = new SalesService(dbContext);

            var sales = await service.GetMonthlySales(2021);

            Assert.NotNull(sales);
            Assert.Equal(1000, sales.Jun);

            sales = await service.GetMonthlySales(2022);

            Assert.NotNull(sales);
            Assert.Equal(2000, sales.Jun);
        }

        [Fact]
        public async Task ShouldNotReturnMonthlySales()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var year2021 = new MonthlySales(2021);
            year2021.Jun = 1000m;

            var year2022 = new MonthlySales(2022);
            year2022.Jun = 2000m;

            dbContext.MonthlySales.AddRange(year2021, year2022);
            dbContext.SaveChanges();

            var service = new SalesService(dbContext);

            var sales = await service.GetMonthlySales(2023);

            Assert.Null(sales);
        }
    }
}
