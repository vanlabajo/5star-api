using Core;
using Core.Services;
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
    public class InvoiceServiceTests : IDisposable
    {
        private readonly SqliteConnection sqliteConnection;
        private readonly DbContextOptions<FiveStarDbContext> dbContextOptions;

        public InvoiceServiceTests()
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
        public async Task ShouldGetInvoices()
        {
            var invoices = new List<Invoice>
            {
                new Invoice(),
                new Invoice()
            };

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Invoices.AddRange(invoices);
                dbContext.SaveChanges();
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new InvoiceService(dbContext);

                var pagedResult = await service.GetInvoices(new PagedQuery());

                Assert.NotNull(pagedResult);
                Assert.NotEmpty(pagedResult.Data);
                Assert.Equal(2, pagedResult.Data.Count);
                Assert.Equal(2, pagedResult.CollectionSize);
            }
        }

        [Fact]
        public async Task ShouldGetFilteredInvoices()
        {
            var invoice1 = new Invoice();
            var invoice2 = new Invoice();

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Invoices.AddRange(invoice1, invoice2);
                dbContext.SaveChanges();
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new InvoiceService(dbContext);

                var query = new PagedQuery { SearchTerm = invoice2.ReferenceNumber };
                query.PageInfo.PageSize = 40;

                var pagedResult = await service.GetInvoices(query);

                Assert.NotNull(pagedResult);
                Assert.NotEmpty(pagedResult.Data);
                Assert.Single(pagedResult.Data);
                Assert.Equal(1, pagedResult.CollectionSize);
            }
        }

        [Fact]
        public async Task ShouldGetInvoiceByIdOrReferenceNumber()
        {
            var invoice1 = new Invoice();
            var invoice2 = new Invoice();

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                dbContext.Invoices.AddRange(invoice1, invoice2);
                dbContext.SaveChanges();
            }

            using (var dbContext = new FiveStarDbContext(dbContextOptions))
            {
                var service = new InvoiceService(dbContext);

                var invoice = await service.GetInvoice(invoice1.Id);

                Assert.NotNull(invoice);

                invoice = await service.GetInvoice(invoice2.ReferenceNumber);

                Assert.NotNull(invoice);
            }
        }
    }
}
