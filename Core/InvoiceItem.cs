namespace Core
{
    public class InvoiceItem
    {
        public Product Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal Total => Product.Price * Quantity;

        public InvoiceItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        private InvoiceItem() { }

        public void SetQuantity(int quantity)
        {
            if (!quantity.Equals(Quantity))
            {
                Quantity = quantity;
            }
        }
    }
}
