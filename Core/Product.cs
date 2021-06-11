using System;

namespace Core
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Upc { get; private set; }
        public decimal Cost { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public DateTime LastUpdateTimeUtc { get; private set; }
        public string LastUpdatedBy { get; private set; }
        public DateTime CreatedTimeUtc { get; private set; }
        public string CreatedBy { get; private set; }
        public byte[] Timestamp { get; private set; }

        public Product(string createdBy, string name, string upc)
        {
            var timeStamp = DateTime.UtcNow;
            CreatedBy = createdBy;
            CreatedTimeUtc = timeStamp;

            Name = name;
            Upc = upc;
        }

        public void SetName(string updatedBy, string name)
        {
            if (!name.Equals(Name))
            {
                var timeStamp = DateTime.UtcNow;
                LastUpdatedBy = updatedBy;
                LastUpdateTimeUtc = timeStamp;

                Name = name;
            }            
        }

        public void SetUpc(string updatedBy, string upc)
        {
            if (!upc.Equals(Upc))
            {
                var timeStamp = DateTime.UtcNow;
                LastUpdatedBy = updatedBy;
                LastUpdateTimeUtc = timeStamp;

                Upc = upc;
            }            
        }

        public void SetQuantity(string updatedBy, int quantity)
        {
            if (!quantity.Equals(Quantity))
            {
                var timeStamp = DateTime.UtcNow;
                LastUpdatedBy = updatedBy;
                LastUpdateTimeUtc = timeStamp;

                Quantity = quantity;
            }
        }

        public void SetCost(string updatedBy, decimal cost)
        {
            if (!cost.Equals(Cost))
            {
                var timeStamp = DateTime.UtcNow;
                LastUpdatedBy = updatedBy;
                LastUpdateTimeUtc = timeStamp;

                Cost = cost;
            }            
        }

        public void SetPrice(string updatedBy, decimal price)
        {
            if (!price.Equals(Price))
            {
                var timeStamp = DateTime.UtcNow;
                LastUpdatedBy = updatedBy;
                LastUpdateTimeUtc = timeStamp;

                Price = price;
            }            
        }
    }
}
