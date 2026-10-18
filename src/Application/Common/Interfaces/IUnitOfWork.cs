namespace Cfo.Cats.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IApplicationDbContext DbContext { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync();
}
