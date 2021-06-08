using Core;
using Core.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly SqliteConnection sqliteConnection;
        private readonly DbContextOptions<FiveStarDbContext> dbContextOptions;

        public ProductServiceTests()
        {
            sqliteConnection = new SqliteConnection("Filename=:memory:");
            sqliteConnection.Open();

            dbContextOptions = new DbContextOptionsBuilder<FiveStarDbContext>()
                .UseSqlite(sqliteConnection)
                .Options;

            using var dbContext = new FiveStarDbContext(dbContextOptions);
            dbContext.Database.EnsureCreated();

            #region SQLite Implementation of ROWVERSION

            var tables = dbContext.Model.GetEntityTypes();

            foreach (var table in tables)
            {
                var props = table.GetProperties()
                    .Where(p => p.ClrType == typeof(byte[])
                        && p.ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate
                        && p.IsConcurrencyToken);

                var tableName = table.GetTableName();

                foreach (var field in props)
                {
                    string[] SQLs = new string[] {
                                $@"CREATE TRIGGER Set{tableName}_{field.Name}OnUpdate
                                    AFTER UPDATE ON {tableName}
                                    BEGIN
                                        UPDATE {tableName}
                                        SET {field.Name} = randomblob(8)
                                        WHERE rowid = NEW.rowid;
                                    END",
                                    $@"CREATE TRIGGER Set{tableName}_{field.Name}OnInsert
                                    AFTER INSERT ON {tableName}
                                    BEGIN
                                        UPDATE {tableName}
                                        SET {field.Name} = randomblob(8)
                                        WHERE rowid = NEW.rowid;
                                    END"
                                };

                    foreach (var sql in SQLs)
                    {
                        using (var command = sqliteConnection.CreateCommand())
                        {
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            #endregion
        }

        public void Dispose()
        {
            sqliteConnection.Dispose();
        }

        [Fact]
        public async Task ShouldAddProduct()
        {
            using var dbContext = new FiveStarDbContext(dbContextOptions);
            var service = new ProductService(dbContext);

            var product = new Product("tester1", "test", "test");

            var result = await service.AddProduct(product);

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.ValidationErrors);

            product = await service.GetProduct(result.Id.Value);

            Assert.Equal(1, product.Id);
            Assert.Equal("tester1", product.CreatedBy);
            Assert.Equal("test", product.Name);
            Assert.Equal("test", product.Upc);
            Assert.NotNull(product.Timestamp);
        }

        [Fact]
        public async Task ShouldUpdateProduct()
        {
            // Use a different scoped context to simulate two separate requests of
            // 1. Fetching the product, i.e., api/products/1
            // 2. Updating the product, i.e., put api/products

            var product = new Product("tester1", "test", "test");

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Products.Add(product);
                dbContext.SaveChanges();

                Assert.Equal(1, product.Id);
                Assert.Equal(0, product.Quantity);
            }

            product.SetQuantity("tester2", 10);
            
            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var result = await service.UpdateProduct(product);

                Assert.True(result.Success);
                Assert.Empty(result.ValidationErrors);

                var updatedProduct = await service.GetProduct(product.Id);

                Assert.Equal(10, updatedProduct.Quantity);
                Assert.False(product.Timestamp.SequenceEqual(updatedProduct.Timestamp)); // Timestamp should updated externally
            }
        }

        [Fact]
        public async Task ShouldNotAllowConcurrentUpdates()
        {
            // Use a different scoped context to simulate two separate requests of
            // 1. Fetching the product, i.e., api/products/1
            // 2. Updating the product, i.e., put api/products

            var product = new Product("tester1", "test", "test");

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Products.Add(product);
                dbContext.SaveChanges();

                Assert.Equal(1, product.Id);
                Assert.Equal(0, product.Quantity);
            }

            product.SetQuantity("tester2", 10);

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var result = await service.UpdateProduct(product);

                Assert.True(result.Success);
                Assert.Empty(result.ValidationErrors);

                var updatedProduct = await service.GetProduct(product.Id);

                Assert.Equal(10, updatedProduct.Quantity);
                Assert.False(product.Timestamp.SequenceEqual(updatedProduct.Timestamp)); // Timestamp should updated externally
            }

            product.SetQuantity("tester3", 20);

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var result = await service.UpdateProduct(product);

                Assert.False(result.Success);
                Assert.Single(result.ValidationErrors);
                Assert.Equal("The record you are trying to update was modified by another user. If you still want to update this record, please refresh the list and try again.",
                    result.ValidationErrors["Product"]);
            }
        }

        [Fact]
        public async Task ShouldRemoveProduct()
        {
            var product = new Product("tester1", "test", "test");

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Products.Add(product);
                dbContext.SaveChanges();

                Assert.Equal(1, product.Id);
                Assert.Equal(0, product.Quantity);
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var result = await service.RemoveProduct(product.Id);

                Assert.True(result.Success);
                Assert.Empty(result.ValidationErrors);

                var removeProduct = await service.GetProduct(product.Id);

                Assert.Null(removeProduct);
            }
        }

        [Fact]
        public async Task ShouldGetProducts()
        {
            var products = new List<Product>
            {
                new Product("tester1", "test1", "test1"),
                new Product("tester2", "test2", "test2")
            };

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.SaveChanges();
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var pagedResult = await service.GetProducts(new PagedQuery());

                Assert.NotNull(pagedResult);
                Assert.NotEmpty(pagedResult.Data);
                Assert.Equal(2, pagedResult.Data.Count);
                Assert.Equal(1, pagedResult.PageCount);
                Assert.Equal(30, pagedResult.PageSize);
                Assert.Equal(2, pagedResult.Total);
            }
        }

        [Fact]
        public async Task ShouldGetFilteredProducts()
        {
            var products = new List<Product>
            {
                new Product("tester1", "test1", "test1"),
                new Product("tester2", "test2", "test2")
            };

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.SaveChanges();
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var query = new PagedQuery { SearchTerm = "test2" };
                query.PageInfo.PageSize = 40;

                var pagedResult = await service.GetProducts(query);

                Assert.NotNull(pagedResult);
                Assert.NotEmpty(pagedResult.Data);
                Assert.Single(pagedResult.Data);
                Assert.Equal(1, pagedResult.PageCount);
                Assert.Equal(40, pagedResult.PageSize);
                Assert.Equal(1, pagedResult.Total);
            }
        }

        [Fact]
        public async Task ShouldGetProductByNameOrUpc()
        {
            var products = new List<Product>
            {
                new Product("tester1", "name1", "upc1"),
                new Product("tester2", "name2", "upc2")
            };

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Products.AddRange(products);
                dbContext.SaveChanges();
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new ProductService(dbContext);

                var product = await service.GetProduct("name1");

                Assert.NotNull(product);

                product = await service.GetProduct("upc2");

                Assert.NotNull(product);
            }
        }
    }
}
