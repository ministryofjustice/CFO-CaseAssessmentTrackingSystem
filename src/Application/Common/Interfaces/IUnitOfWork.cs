namespace Cfo.Cats.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IApplicationDbContext DbContext { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
