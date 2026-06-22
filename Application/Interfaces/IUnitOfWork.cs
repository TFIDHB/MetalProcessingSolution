namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        IMetalServiceRepository MetalServices { get; }
        IUnliquidProductRepository UnliquidProducts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IUserRepository Users { get; }
    }
}
