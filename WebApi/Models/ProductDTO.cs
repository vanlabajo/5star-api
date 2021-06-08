namespace WebApi.Models
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Upc { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
