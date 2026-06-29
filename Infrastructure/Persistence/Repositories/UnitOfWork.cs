using Application.Interfaces;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IMetalServiceRepository MetalServices { get; }
        public IProductRepository Products { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            MetalServices = new MetalServiceRepository(_context);
            Products = new ProductRepository(_context);
            Users = new UserRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}