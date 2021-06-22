using Core;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
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
    }
}
