using Core;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> logger;
        private readonly IProductService productService;

        public ProductsController(ILogger<ProductsController> logger,
            IProductService productService)
        {
            this.logger = logger;
            this.productService = productService;
        }

        [HttpGet]
        public async Task<PagedResult<Product>> Get([FromQuery] PagedQuery pagedQuery)
        {
            return await productService.GetProducts(pagedQuery);
        }

        [HttpGet("{id}")]
        public async Task<Product> Get(int id)
        {
            return await productService.GetProduct(id);
        }

        [HttpGet("{nameOrUpc}")]
        public async Task<Product> Get(string nameOrUpc)
        {
            return await productService.GetProduct(nameOrUpc);
        }

        [HttpPost]
        public async Task<ServiceResult> Post([FromBody] ProductDTO product)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var newProduct = new Product(userId, product.Name, product.Upc);
            newProduct.SetCost(userId, product.Cost);
            newProduct.SetPrice(userId, product.Price);
            newProduct.SetQuantity(userId, product.Quantity);

            var serviceResult = await productService.AddProduct(newProduct);
            if (serviceResult.Success)
                logger.LogInformation($"{userId} added a new product with name: {newProduct.Name} and upc: {newProduct.Upc}");

            return serviceResult;
        }

        [HttpPut("{id}")]
        public async Task<ServiceResult> Put(int id, [FromBody] ProductDTO product)
        {
            var data = await productService.GetProduct(id);
            if (data != null)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                data.SetName(userId, product.Name);
                data.SetUpc(userId, product.Upc);
                data.SetCost(userId, product.Cost);
                data.SetPrice(userId, product.Price);
                data.SetQuantity(userId, product.Quantity);

                var serviceResult = await productService.UpdateProduct(data);
                if (serviceResult.Success)
                    logger.LogInformation($"{userId} updated the product with name: {data.Name} and upc: {data.Upc}");

                return serviceResult;
            }
            else
            {
                var result = new ServiceResult();
                result.ValidationErrors.Add("Product", "Product does not exist");
                return result;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ServiceResult> Delete(int id)
        {
            var data = await productService.GetProduct(id);
            if (data != null)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var serviceResult = await productService.RemoveProduct(id);
                if (serviceResult.Success)
                    logger.LogInformation($"{userId} deleted the product with name: {data.Name} and upc: {data.Upc}");

                return serviceResult;
            }
            else
            {
                var result = new ServiceResult();
                result.ValidationErrors.Add("Product", "Product does not exist");
                return result;
            }
        }
    }
}
