using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
           => await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await context.Users.FindAsync([id], cancellationToken);

        public async Task AddAsync(User user, CancellationToken cancellationToken)
            => await context.Users.AddAsync(user, cancellationToken);

        public void Update(User user)
            => context.Users.Update(user);

        public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken)
            => await context.Users.AnyAsync(u => u.Email == email, cancellationToken);

        public async Task<bool> AnyAdminExistsAsync(CancellationToken cancellationToken)
            => await context.Users.AnyAsync(u => u.Role == UserRole.Admin, cancellationToken);
    }
}
