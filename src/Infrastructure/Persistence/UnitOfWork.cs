using Microsoft.EntityFrameworkCore.Storage;

namespace Cfo.Cats.Infrastructure.Persistence;

public class UnitOfWork(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IUnitOfWork
{
    private IApplicationDbContext? _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public IApplicationDbContext DbContext => _dbContext ??= dbContextFactory.CreateDbContext();
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await ((DbContext)DbContext).SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return;
        }
        _currentTransaction = await DbContext.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        try
        {
            await ((DbContext)DbContext).SaveChangesAsync(CancellationToken.None);
            if (_currentTransaction is not null)
            {
                await _currentTransaction.CommitAsync();    
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.RollbackAsync();    
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}
