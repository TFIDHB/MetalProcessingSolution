namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public ProductCategory Category { get; set; }
        public List<ProductImage> Images { get; set; } = [];
    }
}