using Core;
using System;
using System.Linq;
using Xunit;

namespace Tests.UnitTests
{
    public class InvoiceTests
    {
        [Fact]
        public void ShouldPopulateReferenceNumber()
        {
            var invoice = new Invoice();

            Assert.NotNull(invoice.ReferenceNumber);
            Assert.NotEmpty(invoice.ReferenceNumber);
            Assert.Equal(6, invoice.ReferenceNumber.Length);
        }

        [Fact]
        public void ShouldPopulateCreationDate()
        {
            var invoice = new Invoice();

            Assert.NotEqual(DateTime.MinValue, invoice.CreatedTime);
        }

        [Fact]
        public void ShouldAddItem()
        {
            var invoice = new Invoice();
            var product = new Product("tester1", "abc", "1001");
            product.SetPrice("tester1", 100);
            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product, 1, null);

            invoice.AddItem(new InvoiceItem(product, 1));

            Assert.Single(invoice.Items);
            Assert.Equal(1, invoice.Items.First().Quantity);
            Assert.Equal(100, invoice.Items.First().Total);
            Assert.Equal(100, invoice.Total);
        }

        [Fact]
        public void ShouldAddQuantityForDuplicateItem()
        {
            var invoice = new Invoice();
            var product = new Product("tester1", "abc", "1001");
            product.SetPrice("tester1", 100);

            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product, 1, null);

            invoice.AddItem(new InvoiceItem(product, 10));

            invoice.AddItem(new InvoiceItem(product, 1));

            Assert.Single(invoice.Items);
            Assert.Equal(11, invoice.Items.First().Quantity);
            Assert.Equal(1100, invoice.Items.First().Total);
            Assert.Equal(1100, invoice.Total);
        }

        [Fact]
        public void ShouldReturnTheCorrectTotalForInvoiceItem()
        {
            var invoice = new Invoice();
            var product = new Product("tester1", "abc", "1001");
            product.SetPrice("tester1", 100);

            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product, 1, null);

            invoice.AddItem(new InvoiceItem(product, 10));

            Assert.Single(invoice.Items);
            Assert.Equal(1000, invoice.Items.First().Total);
        }

        [Fact]
        public void ShouldReturnTheCorrectTotalForInvoice()
        {
            var invoice = new Invoice();

            var product1 = new Product("tester1", "abc", "1001");
            product1.SetPrice("tester1", 100);
            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product1, 1, null);

            var product2 = new Product("tester1", "zxc", "1002");
            product2.SetPrice("tester1", 200);
            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product2, 2, null);

            invoice.AddItem(new InvoiceItem(product1, 2));
            invoice.AddItem(new InvoiceItem(product2, 3));

            Assert.Equal(2, invoice.Items.Count);
            Assert.Equal(800, invoice.Total);
        }

        [Fact]
        public void ShouldRemoveItem()
        {
            var invoice = new Invoice();

            var product1 = new Product("tester1", "abc", "1001");
            product1.SetPrice("tester1", 100);
            typeof(Product).GetProperty(nameof(Product.Id)).SetValue(product1, 1, null);

            var invoiceItem = new InvoiceItem(product1, 1);

            invoice.AddItem(invoiceItem);

            Assert.Single(invoice.Items);
            Assert.Equal(100, invoice.Total);

            invoice.RemoveItem(invoiceItem);
            Assert.Empty(invoice.Items);
            Assert.Equal(0, invoice.Total);
        }
    }
}
