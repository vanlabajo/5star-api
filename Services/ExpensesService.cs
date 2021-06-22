using Core;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Threading.Tasks;

namespace Services
{
    public class ExpensesService : IExpensesService
    {
        private readonly FiveStarDbContext dbContext;

        public ExpensesService(FiveStarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<MonthlyExpenses> GetMonthlyExpenses(int year)
        {
            var expenses = await dbContext.MonthlyExpenses
                .FirstOrDefaultAsync(s => s.Year == year);
            return expenses;
        }
    }
}
