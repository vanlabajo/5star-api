using Core;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class SalesService : ISalesService
    {
        private readonly FiveStarDbContext dbContext;

        public SalesService(FiveStarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<MonthlySales> GetMonthlySales(int year)
        {
            var sales = await dbContext.MonthlySales
                .FirstOrDefaultAsync(s => s.Year == year);
            return sales;
        }

        public async Task<decimal> GetTotalSalesToday()
        {
            var invoices = await dbContext.Invoices
                .Where(i => i.CreatedTime >= DateTime.Today)
                .Include(i => i.Items).ThenInclude(i => i.Product)
                .ToListAsync();

            var sales = invoices.Sum(i => i.Total);

            return sales;
        }
    }
}
