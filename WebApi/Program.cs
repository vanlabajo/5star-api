using Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            DbSeeding(host);

            host.Run();
        }

        private static void DbSeeding(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<FiveStarDbContext>();
                //context.Database.EnsureCreated();

                if (!context.MonthlySales.Any())
                {
                    context.MonthlySales.Add(new MonthlySales(2021)
                    {
                        Apr = 85000,
                        May = 60000,
                        Jun = 100000
                    });

                    context.SaveChanges();
                }


                if (!context.MonthlyExpenses.Any())
                {
                    context.MonthlyExpenses.Add(new MonthlyExpenses(2021)
                    {
                        Apr = 80000,
                        May = 60000,
                        Jun = 90000
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog((context, configuration) =>
                        {
                            configuration
                                .MinimumLevel.Debug()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                .MinimumLevel.Override("System", LogEventLevel.Warning)
                                .Enrich.FromLogContext()
                                //.WriteTo.File(@"WebApi.Log.txt", rollOnFileSizeLimit: true, fileSizeLimitBytes: 10485760)
                                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
                        });
                });
    }
}
