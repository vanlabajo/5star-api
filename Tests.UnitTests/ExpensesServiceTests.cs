using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.UnitTests
{
    public class ExpensesServiceTests : IDisposable
    {
        private readonly SqliteConnection sqliteConnection;
        private readonly DbContextOptions<FiveStarDbContext> dbContextOptions;

        public ExpensesServiceTests()
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
        public async Task ShouldReturnMonthlyExpenses()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var year2021 = new MonthlyExpenses(2021);
            year2021.Jun = 1000m;

            var year2022 = new MonthlyExpenses(2022);
            year2022.Jun = 2000m;

            dbContext.MonthlyExpenses.AddRange(year2021, year2022);
            dbContext.SaveChanges();

            var service = new ExpensesService(dbContext);

            var Expenses = await service.GetMonthlyExpenses(2021);

            Assert.NotNull(Expenses);
            Assert.Equal(1000, Expenses.Jun);

            Expenses = await service.GetMonthlyExpenses(2022);

            Assert.NotNull(Expenses);
            Assert.Equal(2000, Expenses.Jun);
        }

        [Fact]
        public async Task ShouldNotReturnMonthlyExpenses()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var year2021 = new MonthlyExpenses(2021);
            year2021.Jun = 1000m;

            var year2022 = new MonthlyExpenses(2022);
            year2022.Jun = 2000m;

            dbContext.MonthlyExpenses.AddRange(year2021, year2022);
            dbContext.SaveChanges();

            var service = new ExpensesService(dbContext);

            var Expenses = await service.GetMonthlyExpenses(2023);

            Assert.Null(Expenses);
        }
    }
}
