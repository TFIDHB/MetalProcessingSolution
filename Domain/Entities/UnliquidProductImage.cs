namespace Domain.Entities
{
    public class UnliquidProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int UnliquidProductId { get; set; }
        public UnliquidProduct? UnliquidProduct { get; set; }
    }
}
