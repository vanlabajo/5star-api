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

namespace UnitTests
{
    public class CartServiceTests : IDisposable
    {
        private readonly SqliteConnection sqliteConnection;
        private readonly DbContextOptions<FiveStarDbContext> dbContextOptions;

        public CartServiceTests()
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
        public async Task ShouldCheckout()
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
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);
        }

        [Fact]
        public async Task ShouldDeductFromStockUponSuccessfulCheckout()
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

            invoiceItems.Add(new InvoiceItem(products[0], 4));
            invoiceItems.Add(new InvoiceItem(products[1], 7));

            await service.Checkout(invoiceItems);

            var p1 = dbContext.Products.Find(products[0].Id);
            Assert.Equal(6, p1.Quantity);

            var p2 = dbContext.Products.Find(products[1].Id);
            Assert.Equal(3, p2.Quantity);
        }

        [Fact]
        public async Task ShouldGenerateTheCorrectInvoice()
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

            await service.Checkout(invoiceItems);

            var invoices = dbContext.Invoices.ToList();

            Assert.Single(invoices);

            var invoice = invoices[0];

            Assert.Equal(2, invoice.Items.Count);
            Assert.All(invoice.Items, item => Assert.Equal(item.Product.Price * item.Quantity, item.Total));
            Assert.Equal(16, invoice.Total);
        }

        [Fact]
        public async Task ShouldValidateIfProductIsValid()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var product1 = new Product("tester1", "abc", "abc");
            product1.SetQuantity("tester1", 10);
            product1.SetPrice("tester2", 1.5m);
            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product1, 1, null); // does not exist in DB

            var product2 = new Product("tester1", "zxc", "zxc");
            product2.SetQuantity("tester1", 10);
            product2.SetPrice("tester2", 1.7m);
            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product2, 2, null); // does not exist in DB


            var service = new CartService(dbContext);

            var invoiceItems = new List<InvoiceItem>();

            invoiceItems.Add(new InvoiceItem(product1, 5));
            invoiceItems.Add(new InvoiceItem(product2, 5));

            var result = await service.Checkout(invoiceItems);

            Assert.False(result.Success);
            Assert.Null(result.Id);
            Assert.True(result.ValidationErrors.ContainsKey("1"));
            Assert.Equal("Product is not valid.", result.ValidationErrors["1"]);
            Assert.True(result.ValidationErrors.ContainsKey("2"));
            Assert.Equal("Product is not valid.", result.ValidationErrors["2"]);
        }

        [Fact]
        public async Task ShouldValidateIfProductIsOutOfStock()
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

            invoiceItems.Add(new InvoiceItem(products[0], 15));
            invoiceItems.Add(new InvoiceItem(products[1], 5));

            var result = await service.Checkout(invoiceItems);

            Assert.False(result.Success);
            Assert.Null(result.Id);
            Assert.True(result.ValidationErrors.ContainsKey("1"));
            Assert.Equal("Product is out of stock.", result.ValidationErrors["1"]);
            Assert.False(result.ValidationErrors.ContainsKey("2"));
        }

        [Fact]
        public async Task ShouldGenerateTheCorrectSales()
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

            await service.Checkout(invoiceItems);

            var invoice = dbContext.Invoices.First();

            var timeStamp = DateTime.UtcNow;
            var sales = dbContext.MonthlySales.FirstOrDefault(s => s.Year == timeStamp.Year);

            Assert.NotNull(sales);
            Assert.Equal(16, (decimal)sales.GetType().GetProperty(timeStamp.ToString("MMM")).GetValue(sales, null));
        }

        [Fact]
        public async Task ShouldValidateIfItemListIsEmpty()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);

            var service = new CartService(dbContext);

            var invoiceItems = new List<InvoiceItem>();

            var result = await service.Checkout(invoiceItems);

            Assert.False(result.Success);
            Assert.Null(result.Id);
            Assert.True(result.ValidationErrors.ContainsKey("Items"));
            Assert.Equal("Item list is empty.", result.ValidationErrors["Items"]);
        }
    }
}
