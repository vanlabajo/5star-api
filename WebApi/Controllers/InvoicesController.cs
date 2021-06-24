using Core;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<PagedResult<Invoice>> Get([FromQuery] PagedQuery pagedQuery)
        {
            return await invoiceService.GetInvoices(pagedQuery);
        }

        [HttpGet("{id:int}")]
        public async Task<Invoice> Get(int id)
        {
            return await invoiceService.GetInvoice(id);
        }

        [HttpGet("search/{referenceNumber}")]
        public async Task<Invoice> Get(string referenceNumber)
        {
            return await invoiceService.GetInvoice(referenceNumber);
        }

    }
}
