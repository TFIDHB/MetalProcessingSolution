using Application.Interfaces;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IMetalServiceRepository MetalServices { get; }
    public IUnliquidProductRepository UnliquidProducts { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        MetalServices = new MetalServiceRepository(_context);
        UnliquidProducts = new UnliquidProductRepository(_context);
        Users = new UserRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
