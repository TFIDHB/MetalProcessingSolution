using Domain.Entities;

namespace Application.Interfaces;

public interface IMetalServiceRepository
{
    Task<IEnumerable<MetalService>> GetAllAsync(CancellationToken cancellationToken);
    Task<MetalService?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(MetalService service, CancellationToken cancellationToken);
    void Update(MetalService service);
    void Delete(MetalService service);
}
