namespace Application.DTOs;

public class MetalServiceResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PriceFrom { get; set; }
    public List<MetalServiceImageResponseDto> Images { get; set; } = [];
}
