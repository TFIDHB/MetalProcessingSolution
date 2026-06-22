using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        void Update(User user);
        Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> AnyAdminExistsAsync(CancellationToken cancellationToken);
    }
}
