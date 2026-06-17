namespace Domain.Entities
{
    public class UnliquidProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "/images/no-image.png";
    }
}
