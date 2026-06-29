using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UnliquidProductRepository(AppDbContext context) : IUnliquidProductRepository
{
    public async Task<IEnumerable<UnliquidProduct>> GetAllAsync(CancellationToken cancellationToken)
        => await context.UnliquidProducts.Include(x => x.Images).ToListAsync(cancellationToken);

    public async Task<UnliquidProduct?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => await context.UnliquidProducts.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(UnliquidProduct product, CancellationToken cancellationToken)
        => await context.UnliquidProducts.AddAsync(product, cancellationToken);

    public void Update(UnliquidProduct product) => context.UnliquidProducts.Update(product);
    public void Delete(UnliquidProduct product) => context.UnliquidProducts.Remove(product);
}
