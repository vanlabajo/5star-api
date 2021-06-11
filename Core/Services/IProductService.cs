using System.Threading.Tasks;

namespace Core.Services
{
    public interface IProductService
    {
        Task<ServiceResult> AddProduct(Product product);
        Task<ServiceResult> UpdateProduct(Product product);
        Task<ServiceResult> RemoveProduct(int productId);
        Task<Product> GetProduct(int productId);
        Task<Product> GetProduct(string nameOrUpc);
        Task<PagedResult<Product>> GetProducts(PagedQuery pagedQuery);
    }
}
