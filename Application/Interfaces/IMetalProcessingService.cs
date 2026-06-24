using Application.DTOs;

namespace Application.Interfaces
{
    public interface IMetalProcessingService
    {
        Task<IEnumerable<UnliquidProductResponseDto>> GetUnliquidProductsAsync(CancellationToken cancellationToken);
        Task<int> CreateUnliquidAsync(UnliquidProductDto dto, CancellationToken cancellationToken);
        Task UpdateUnliquidAsync(int id, UnliquidProductDto dto, CancellationToken cancellationToken);
        Task DeleteUnliquidAsync(int id, CancellationToken cancellationToken);

        Task<IEnumerable<MetalServiceResponseDto>> GetServicesAsync(CancellationToken cancellationToken);
        Task<int> CreateServiceAsync(MetalServiceDto dto, CancellationToken cancellationToken);
        Task UpdateServiceAsync(int id, MetalServiceDto dto, CancellationToken cancellationToken);
        Task DeleteServiceAsync(int id, CancellationToken cancellationToken);

        Task AdjustServicePricesAsync(PriceAdjustmentDto dto, CancellationToken cancellationToken);
        Task AdjustUnliquidPricesAsync(PriceAdjustmentDto dto, CancellationToken cancellationToken);
        Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken);
    }
}
