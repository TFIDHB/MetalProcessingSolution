namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        IMetalServiceRepository MetalServices { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
