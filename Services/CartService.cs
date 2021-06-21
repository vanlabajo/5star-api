using Core;
using Core.Services;
using Repository;
using System;
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

            var timeStamp = DateTime.UtcNow;
            var sales = await dbContext.MonthlySales.FindAsync(timeStamp.Year);

            var property = typeof(MonthlySales).GetProperty(timeStamp.ToString("MMM"));
            if (property != null)
            {
                if (sales != null)
                {
                    var currentValue = (decimal?)property.GetValue(sales, null);
                    property.SetValue(sales, (currentValue ?? 0m) + newInvoice.Total, null);

                    dbContext.MonthlySales.Update(sales);
                }
                else
                {
                    sales = new MonthlySales(timeStamp.Year);
                    property.SetValue(sales, newInvoice.Total, null);
                    dbContext.MonthlySales.Add(sales);
                }
            }

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
