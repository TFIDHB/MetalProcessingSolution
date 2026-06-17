using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class MetalServiceRepository(AppDbContext context) : IMetalServiceRepository
    {
        public async Task<IEnumerable<MetalService>> GetAllAsync(CancellationToken cancellationToken)
            => await context.Services.ToListAsync(cancellationToken);

        public async Task<MetalService?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await context.Services.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task AddAsync(MetalService service, CancellationToken cancellationToken)
            => await context.Services.AddAsync(service, cancellationToken);

        public void Update(MetalService service) => context.Services.Update(service);
        public void Delete(MetalService service) => context.Services.Remove(service);
    }
}