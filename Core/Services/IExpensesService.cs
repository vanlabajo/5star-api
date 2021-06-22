using System.Threading.Tasks;

namespace Core.Services
{
    public interface IExpensesService
    {
        Task<MonthlyExpenses> GetMonthlyExpenses(int year);
    }
}
