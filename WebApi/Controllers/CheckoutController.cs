using Core;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ILogger<CheckoutController> logger;
        private readonly ICartService cartService;
        private readonly IProductService productService;

        public CheckoutController(ILogger<CheckoutController> logger,
            ICartService cartService,
            IProductService productService)
        {
            this.logger = logger;
            this.cartService = cartService;
            this.productService = productService;
        }

        [HttpPost]
        public async Task<ServiceResult> Post([FromBody] List<CartItemDTO> cartItems)
        {
            var userId = User.Identity.Name;
            var newItems = new List<InvoiceItem>();

            foreach (var item in cartItems)
            {
                var product = await productService.GetProduct(item.ProductId);
                newItems.Add(new InvoiceItem(product, item.Quantity));
            }

            var serviceResult = await cartService.Checkout(newItems);
            if (serviceResult.Success)
                logger.LogInformation($"{userId} successfully checked out the following: { string.Join(", ", newItems.Select(i => i.Product?.Name ?? "Unknown" + " x " + i.Quantity)) }");

            return serviceResult;
        }
    }
}
