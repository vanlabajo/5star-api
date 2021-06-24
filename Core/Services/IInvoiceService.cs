using System.Threading.Tasks;

namespace Core.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> GetInvoice(int invoiceId);
        Task<Invoice> GetInvoice(string referenceNumber);
        Task<PagedResult<Invoice>> GetInvoices(PagedQuery pagedQuery);
    }
}
