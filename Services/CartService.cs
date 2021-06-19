using Core;
using Core.Services;
using Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly FiveStarDbContext dbContext;

        public CartService(FiveStarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ServiceResult> Checkout(List<InvoiceItem> items)
        {
            var result = await ValidateCart(items);
            if (result.ValidationErrors.Count > 0) return result;

            var newInvoice = new Invoice();

            foreach (var item in items)
            {
                newInvoice.AddItem(item);

                var product = await dbContext.Products.FindAsync(item.Product.Id);
                product.SetQuantity(null, product.Quantity - item.Quantity);

                dbContext.Products.Update(product);
            }

            dbContext.Invoices.Add(newInvoice);

            result.Success = await dbContext.SaveChangesAsync() > 0;

            if (result.Success) result.Id = newInvoice.Id;
            return result;
        }

        private async Task<ServiceResult> ValidateCart(List<InvoiceItem> items)
        {
            var result = new ServiceResult();

            foreach (var item in items)
            {
                var product = await dbContext.Products.FindAsync(item.Product.Id);
                if (product == null)
                {
                    result.ValidationErrors.Add($"{item.Product.Id}", "Product is not valid.");
                }
                else if (product != null && product.Quantity < item.Quantity)
                {
                    result.ValidationErrors.Add($"{item.Product.Id}", "Product is out of stock.");
                }
            }

            return result;
        }
    }
}
