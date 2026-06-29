using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, CancellationToken cancellationToken)
            => await context.Products
                .Include(p => p.Images)
                .Where(p => p.Category == category)
                .ToListAsync(cancellationToken);

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
            => await context.Products.AddAsync(product, cancellationToken);

        public void Update(Product product)
            => context.Products.Update(product);

        public void Delete(Product product)
            => context.Products.Remove(product);
    }
}