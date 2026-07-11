namespace Application.DTOs;

public class UnliquidProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Quantity { get; set; } = string.Empty;
    public List<UnliquidProductImageResponseDto> Images { get; set; } = [];
}
