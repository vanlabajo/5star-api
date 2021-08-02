using Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Data.Common;

namespace Tests.IntegrationTests
{
    public class SharedSqlServerDatabaseFixture : IDatabaseFixture, IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public SharedSqlServerDatabaseFixture()
        {
            DbConnection = new SqlConnection(@"Server=(local);Database=FiveStarIntegrationTests;Trusted_Connection=True;MultipleActiveResultSets=True;");

            Seed();

            DbConnection.Open();
        }

        public DbConnection DbConnection { get; }

        public FiveStarDbContext CreateDbContext(DbTransaction transaction = null)
        {
            var options = new DbContextOptionsBuilder<FiveStarDbContext>()
                .UseSqlServer(DbConnection)
                .Options;

            var context = new FiveStarDbContext(options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    var options = new DbContextOptionsBuilder<SqlServerFiveStarDbContext>()
                        .UseSqlServer(DbConnection)
                        .Options;

                    using (var context = new SqlServerFiveStarDbContext(options))
                    {
                        context.Database.EnsureDeleted();
                        context.Database.Migrate();

                        context.MonthlySales.Add(new MonthlySales(2021)
                        {
                            Apr = 85000,
                            May = 60000,
                            Jun = 100000
                        });

                        context.MonthlyExpenses.Add(new MonthlyExpenses(2021)
                        {
                            Apr = 80000,
                            May = 60000,
                            Jun = 90000
                        });

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose() => DbConnection.Dispose();
    }
}
