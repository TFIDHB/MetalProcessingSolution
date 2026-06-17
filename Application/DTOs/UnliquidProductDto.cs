using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class UnliquidProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
    }
}
