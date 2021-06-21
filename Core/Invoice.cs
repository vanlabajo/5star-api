using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Invoice
    {
        public int Id { get; private set; }
        public string ReferenceNumber { get; private set; }
        public DateTime CreatedTime { get; private set; }

        private List<InvoiceItem> _items;
        public IReadOnlyCollection<InvoiceItem> Items => _items;

        public decimal Total => _items.Sum(i => i.Total);

        public Invoice()
        {
            CreatedTime = DateTime.UtcNow;
            _items = new List<InvoiceItem>();

            var random = new Random(Guid.NewGuid().GetHashCode());
            var referenceNumber = random.Next(0, 1000000);

            ReferenceNumber = referenceNumber.ToString("D6");
        }

        public void AddItem(InvoiceItem invoiceItem)
        {
            var item = _items.Find(i => i.Product.Id == invoiceItem.Product.Id);
            if (item == null)
            {
                _items.Add(invoiceItem);
            }
            else
            {
                item.SetQuantity(item.Quantity + invoiceItem.Quantity);
            }
        }

        public void RemoveItem(InvoiceItem invoiceItem)
        {
            var item = _items.Find(i => i.Product.Id == invoiceItem.Product.Id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }
    }
}
