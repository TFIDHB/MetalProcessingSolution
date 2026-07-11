using Application.DTOs;

namespace Application.Interfaces
{
    public interface IMetalProcessingService
    {
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken);
        Task<int> CreateProductAsync(ProductDto dto, CancellationToken cancellationToken);
        Task UpdateProductAsync(int id, ProductDto dto, CancellationToken cancellationToken);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken);

        Task<IEnumerable<MetalServiceResponseDto>> GetServicesAsync(CancellationToken cancellationToken);
        Task<int> CreateServiceAsync(MetalServiceDto dto, CancellationToken cancellationToken);
        Task UpdateServiceAsync(int id, MetalServiceDto dto, CancellationToken cancellationToken);
        Task DeleteServiceAsync(int id, CancellationToken cancellationToken);

        Task AdjustServicePricesAsync(PriceAdjustmentDto dto, CancellationToken cancellationToken);
        Task AdjustProductPricesAsync(string category, PriceAdjustmentDto dto, CancellationToken cancellationToken);
        Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken);
    }
}