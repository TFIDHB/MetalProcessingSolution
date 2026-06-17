using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class MetalServiceDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double PriceFrom { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
