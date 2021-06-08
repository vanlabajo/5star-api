using Core;
using System;
using Xunit;

namespace UnitTests
{
    public class ProductTests
    {
        [Fact]
        public void ShouldPopulateCreationByAndCreationDate()
        {
            var product = new Product("tester1", "test", "test");

            Assert.Equal("tester1", product.CreatedBy);
            Assert.NotEqual(DateTime.MinValue, product.CreatedTimeUtc);

            Assert.Equal("test", product.Name);
            Assert.Equal("test", product.Upc);
        }

        [Fact]
        public void ShouldPopulateLastUpdatedByAndUpdateDate()
        {
            var product = new Product("tester1", "test", "test");

            product.SetQuantity("tester2", 1);

            Assert.Equal("tester2", product.LastUpdatedBy);
            Assert.NotEqual(DateTime.MinValue, product.LastUpdateTimeUtc);

            Assert.Equal(1, product.Quantity);
        }

        [Fact]
        public void ShouldUpdatePrice()
        {
            var product = new Product("tester1", "test", "test");

            product.SetPrice("tester2", 1);

            Assert.Equal("tester2", product.LastUpdatedBy);
            Assert.NotEqual(DateTime.MinValue, product.LastUpdateTimeUtc);

            Assert.Equal(1, product.Price);
        }

        [Fact]
        public void ShouldUpdateCost()
        {
            var product = new Product("tester1", "test", "test");

            product.SetCost("tester2", 5);

            Assert.Equal("tester2", product.LastUpdatedBy);
            Assert.NotEqual(DateTime.MinValue, product.LastUpdateTimeUtc);

            Assert.Equal(5, product.Cost);
        }

        [Fact]
        public void ShouldUpdateName()
        {
            var product = new Product("tester1", "test", "test");

            product.SetName("tester2", "abc");

            Assert.Equal("tester2", product.LastUpdatedBy);
            Assert.NotEqual(DateTime.MinValue, product.LastUpdateTimeUtc);

            Assert.Equal("abc", product.Name);
        }

        [Fact]
        public void ShouldUpdateUpc()
        {
            var product = new Product("tester1", "test", "test");

            product.SetUpc("tester2", "abc");

            Assert.Equal("tester2", product.LastUpdatedBy);
            Assert.NotEqual(DateTime.MinValue, product.LastUpdateTimeUtc);

            Assert.Equal("abc", product.Upc);
        }
    }
}
