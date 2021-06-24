using Core;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly FiveStarDbContext dbContext;

        public InvoiceService(FiveStarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Invoice> GetInvoice(int invoiceId)
        {
            var invoice = await dbContext.Invoices
                .Include(i => i.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
            return invoice;
        }

        public async Task<Invoice> GetInvoice(string referenceNumber)
        {
            var invoice = await dbContext.Invoices
                .Include(i => i.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(i => i.ReferenceNumber == referenceNumber);
            return invoice;
        }

        public async Task<PagedResult<Invoice>> GetInvoices(PagedQuery pagedQuery)
        {
            var baseQuery = dbContext.Invoices
                .Include(i => i.Items).ThenInclude(i => i.Product)
                .AsQueryable();
            if (!string.IsNullOrEmpty(pagedQuery.SearchTerm))
                baseQuery = baseQuery.Where(i => i.ReferenceNumber.ToLower().Contains(pagedQuery.SearchTerm.ToLower()));

            var model = await baseQuery
                .OrderByDescending(i => i.CreatedTime)
                .Skip(pagedQuery.PageInfo.Skip)
                .Take(pagedQuery.PageInfo.PageSize)
                .ToListAsync();

            return new PagedResult<Invoice>
            {
                Data = model,
                CollectionSize = baseQuery.Count()
            };
        }
    }
}
