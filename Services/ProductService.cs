using Core;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly FiveStarDbContext dbContext;

        public ProductService(FiveStarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ServiceResult> AddProduct(Product product)
        {
            var result = await ValidateProduct(product);
            if (result.ValidationErrors.Count > 0) return result;

            var newProduct = new Product(product.CreatedBy, product.Name, product.Upc);
            newProduct.SetCost(product.CreatedBy, product.Cost);
            newProduct.SetPrice(product.CreatedBy, product.Price);
            newProduct.SetQuantity(product.CreatedBy, product.Quantity);

            dbContext.Products.Add(newProduct);
            result.Success = await dbContext.SaveChangesAsync() > 0;

            if (result.Success) result.Id = newProduct.Id;
            return result;
        }

        public async Task<Product> GetProduct(int productId)
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);
            return product;
        }

        public async Task<Product> GetProduct(string nameOrUpc)
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Name.ToLower().Contains(nameOrUpc.ToLower()) 
                    || p.Upc.ToLower().Contains(nameOrUpc.ToLower()));
            return product;
        }

        public async Task<PagedResult<Product>> GetProducts(PagedQuery pagedQuery)
        {
            var baseQuery = dbContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(pagedQuery.SearchTerm))
                baseQuery = baseQuery.Where(p => p.Name.ToLower().Contains(pagedQuery.SearchTerm.ToLower())
                    || p.Upc.ToLower().Contains(pagedQuery.SearchTerm.ToLower()));

            var model = await baseQuery
                .OrderBy(p => p.Name)
                .Skip(pagedQuery.PageInfo.Skip)
                .Take(pagedQuery.PageInfo.PageSize)                
                .ToListAsync();

            return new PagedResult<Product>
            {
                Data = model,
                Total = baseQuery.Count(),
                PageSize = pagedQuery.PageInfo.PageSize
            };
        }

        public async Task<ServiceResult> RemoveProduct(int productId)
        {
            var result = new ServiceResult();

            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                result.ValidationErrors.Add("Product", "Product does not exist");
                return result;
            }

            try
            {
                dbContext.Products.Remove(product);
                result.Success = await dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                if (!(ex.InnerException?.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ?? false)) throw;
            }
            
            return result;
        }

        public async Task<ServiceResult> UpdateProduct(Product product)
        {
            var result = await ValidateProduct(product);
            var data = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

            if (data != null)
            {
                if (!data.Timestamp.SequenceEqual(product.Timestamp))
                    result.ValidationErrors["Product"] = "The record you are trying to update was modified by another user. If you still want to update this record, please refresh the list and try again.";

                if (result.ValidationErrors.Count == 0)
                {
                    data.SetName(product.LastUpdatedBy, product.Name);
                    data.SetUpc(product.LastUpdatedBy, product.Upc);
                    data.SetCost(product.LastUpdatedBy, product.Cost);
                    data.SetPrice(product.LastUpdatedBy, product.Price);
                    data.SetQuantity(product.LastUpdatedBy, product.Quantity);

                    dbContext.Products.Update(data);
                    result.Success = await dbContext.SaveChangesAsync() > 0;
                }
            }
            else
                result.ValidationErrors.Add("Product", "Product does not exist");

            return result;
        }

        private async Task<ServiceResult> ValidateProduct(Product product)
        {
            var result = new ServiceResult();

            if (string.IsNullOrEmpty(product.Name))
                result.ValidationErrors.Add("Name", "Name is required.");

            if (string.IsNullOrEmpty(product.Upc))
                result.ValidationErrors.Add("Upc", "UPC is required.");
            else
            {
                var existingData = await dbContext.Products.FirstOrDefaultAsync(p => p.Upc == product.Upc && p.Id != product.Id);
                if (existingData != null)
                    result.ValidationErrors.Add("Upc", "UPC already exists");
            }

            return result;
        }
    }
}
