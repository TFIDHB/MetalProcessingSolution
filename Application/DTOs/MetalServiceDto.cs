using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public class MetalServiceDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PriceFrom { get; set; }
    public List<IFormFile>? NewImages { get; set; }
    public List<int>? DeleteImageIds { get; set; }
}
