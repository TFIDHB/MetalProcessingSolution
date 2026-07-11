using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<IFormFile>? NewImages { get; set; }
        public List<int>? DeleteImageIds { get; set; }
    }
}