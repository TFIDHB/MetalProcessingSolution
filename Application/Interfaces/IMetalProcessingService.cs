using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMetalProcessingService
    {
        Task<IEnumerable<UnliquidProduct>> GetUnliquidProductsAsync(CancellationToken cancellationToken);
        Task<int> CreateUnliquidAsync(UnliquidProductDto dto, CancellationToken cancellationToken);
        Task UpdateUnliquidAsync(int id, UnliquidProductDto dto, CancellationToken cancellationToken);
        Task DeleteUnliquidAsync(int id, CancellationToken cancellationToken);

        Task<IEnumerable<MetalServiceResponseDto>> GetServicesAsync(CancellationToken cancellationToken);
        Task<int> CreateServiceAsync(MetalServiceDto dto, CancellationToken cancellationToken);
        Task UpdateServiceAsync(int id, MetalServiceDto dto, CancellationToken cancellationToken);
        Task DeleteServiceAsync(int id, CancellationToken cancellationToken);
    }
}
