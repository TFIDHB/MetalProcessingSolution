using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnliquidProductRepository
    {
        Task<IEnumerable<UnliquidProduct>> GetAllAsync(CancellationToken cancellationToken);
        Task<UnliquidProduct?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(UnliquidProduct product, CancellationToken cancellationToken);
        void Update(UnliquidProduct product);
        void Delete(UnliquidProduct product);
    }
}
