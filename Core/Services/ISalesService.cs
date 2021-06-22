using System.Threading.Tasks;

namespace Core.Services
{
    public interface ISalesService
    {
        Task<MonthlySales> GetMonthlySales(int year);
        Task<decimal> GetTotalSalesToday();
    }
}
