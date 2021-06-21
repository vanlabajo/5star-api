using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ICartService
    {
        Task<ServiceResult> Checkout(List<InvoiceItem> items);
    }
}
