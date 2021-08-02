using Core;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Repository;
using System;
using System.Data.Common;

namespace Tests.IntegrationTests
{
    public class SharedMariaDbDatabaseFixture : IDatabaseFixture, IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public SharedMariaDbDatabaseFixture()
        {
            var connectionString = @"server=localhost;user id=root;password=Password21!;port=3306;database=fivestarintegrationtests;";

            DbConnection = new MySqlConnection(connectionString);

            ServerVersion = ServerVersion.AutoDetect(connectionString);

            Seed();

            DbConnection.Open();
        }

        public DbConnection DbConnection { get; }
        public ServerVersion ServerVersion { get; }

        public FiveStarDbContext CreateDbContext(DbTransaction transaction = null)
        {
            var options = new DbContextOptionsBuilder<FiveStarDbContext>()
                .UseMySql(DbConnection, ServerVersion, mysqlOptions =>
                {
                    mysqlOptions.MaxBatchSize(1);
                    mysqlOptions.UseNewtonsoftJson();

                    //if (AppConfig.EfRetryOnFailure > 0)
                    //{
                    //    mysqlOptions.EnableRetryOnFailure(AppConfig.EfRetryOnFailure, TimeSpan.FromSeconds(5), null);
                    //}
                })
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
                    var options = new DbContextOptionsBuilder<MariaDbFiveStarDbContext>()
                        .UseMySql(DbConnection, ServerVersion, mysqlOptions =>
                        {
                            mysqlOptions.MaxBatchSize(1);
                            mysqlOptions.UseNewtonsoftJson();
                        })
                        .Options;

                    using (var context = new MariaDbFiveStarDbContext(options))
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
