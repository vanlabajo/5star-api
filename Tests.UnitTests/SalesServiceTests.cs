using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.UnitTests
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

        [Fact]
        public async Task ShouldReturnTodayTotalSales()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var product1 = new Product("tester1", "abc", "abc");
            product1.SetQuantity("tester1", 10);
            product1.SetPrice("tester2", 1.5m);

            var product2 = new Product("tester1", "zxc", "zxc");
            product2.SetQuantity("tester1", 10);
            product2.SetPrice("tester2", 1.7m);

            dbContext.Products.AddRange(product1, product2);
            dbContext.SaveChanges();

            var service = new CartService(dbContext);

            var products = dbContext.Products.ToList();

            var invoiceItems = new List<InvoiceItem>();

            invoiceItems.Add(new InvoiceItem(products[0], 5));
            invoiceItems.Add(new InvoiceItem(products[1], 5));

            var result = await service.Checkout(invoiceItems);

            Assert.True(result.Success);

            var salesService = new SalesService(dbContext);

            var sales = await salesService.GetTotalSalesToday();

            Assert.Equal(16, sales);
        }

        [Fact]
        public async Task ShouldReturnZeroTotalSales()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var salesService = new SalesService(dbContext);

            var sales = await salesService.GetTotalSalesToday();

            Assert.Equal(0, sales);
        }
    }
}
