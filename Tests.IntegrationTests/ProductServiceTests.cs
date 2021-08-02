using Core;
using Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.IntegrationTests
{
    public abstract class ProductServiceTests
    {
        protected IDatabaseFixture Fixture { get; }

        protected ProductServiceTests(IDatabaseFixture databaseFixture)
        {
            Fixture = databaseFixture;
        }

        [Fact]
        public async Task ShouldAddProduct()
        {
            using var transaction = Fixture.DbConnection.BeginTransaction();

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var service = new ProductService(context);

                var product = new Product("tester1", "test", "test");

                var result = await service.AddProduct(product);

                Assert.True(result.Success);
                Assert.NotEqual(0, result.Id);
                Assert.Empty(result.ValidationErrors);
            }

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var product = context.Products.Single(p => p.Name == "test");

                Assert.NotEqual(0, product.Id);
                Assert.Equal("tester1", product.AuditLog.CreatedBy);
                Assert.Equal("test", product.Name);
                Assert.Equal("test", product.Upc);
                Assert.NotNull(product.TimeStamp);
            }
        }

        [Fact]
        public async Task ShouldUpdateProduct()
        {
            using var transaction = Fixture.DbConnection.BeginTransaction();

            int productId;
            byte[] oldTimeStamp;

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var service = new ProductService(context);

                var product = new Product("tester1", "test", "test");

                var result = await service.AddProduct(product);

                productId = result.Id.Value;

                Assert.True(result.Success);
            }

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var service = new ProductService(context);

                var product = await service.GetProduct("test");

                oldTimeStamp = product.TimeStamp;

                product.SetQuantity("tester2", 10);

                var result = await service.UpdateProduct(product);

                Assert.True(result.Success);
                Assert.Empty(result.ValidationErrors);
            }

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var service = new ProductService(context);

                var product = await service.GetProduct("test");

                Assert.Equal(10, product.Quantity);
                // Timestamp should have been updated externally
                Assert.False(oldTimeStamp.SequenceEqual(product.TimeStamp));
            }
        }

        [Fact]
        public async Task ShouldRemoveProduct()
        {
            using var transaction = Fixture.DbConnection.BeginTransaction();

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var service = new ProductService(context);

                var product = new Product("tester1", "test", "test");

                var result = await service.AddProduct(product);

                Assert.True(result.Success);
            }

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var service = new ProductService(context);

                var product = await service.GetProduct("test");

                var result = await service.RemoveProduct(product.Id);

                Assert.True(result.Success);
                Assert.Empty(result.ValidationErrors);
            }

            using (var context = Fixture.CreateDbContext(transaction))
            {
                var product = context.Products.SingleOrDefault(p => p.Name == "test");

                Assert.Null(product);
            }
        }
    }
}
