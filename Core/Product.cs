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
        public AuditLog AuditLog { get; private set; }
        public byte[] TimeStamp { get; private set; }

        public Product(string createdBy, string name, string upc)
        {
            AuditLog = new AuditLog(createdBy);

            Name = name;
            Upc = upc;
        }

        // For EF core use only
        private Product() { }

        public void SetName(string modifiedBy, string name)
        {
            if (!name.Equals(Name))
            {
                AuditLog.Modified(modifiedBy);

                Name = name;
            }            
        }

        public void SetUpc(string modifiedBy, string upc)
        {
            if (!upc.Equals(Upc))
            {
                AuditLog.Modified(modifiedBy);

                Upc = upc;
            }            
        }

        public void SetQuantity(string modifiedBy, int quantity)
        {
            if (!quantity.Equals(Quantity))
            {
                AuditLog.Modified(modifiedBy);

                Quantity = quantity;
            }
        }

        public void SetCost(string modifiedBy, decimal cost)
        {
            if (!cost.Equals(Cost))
            {
                AuditLog.Modified(modifiedBy);

                Cost = cost;
            }            
        }

        public void SetPrice(string modifiedBy, decimal price)
        {
            if (!price.Equals(Price))
            {
                AuditLog.Modified(modifiedBy);

                Price = price;
            }            
        }
    }
}
