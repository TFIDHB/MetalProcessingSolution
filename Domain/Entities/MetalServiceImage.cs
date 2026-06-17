namespace Domain.Entities
{
    public class MetalServiceImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int MetalServiceId { get; set; }
        public MetalService? MetalService { get; set; }
    }
}
