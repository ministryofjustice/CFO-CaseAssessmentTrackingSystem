namespace Cfo.Cats.Application.Pipeline;

public abstract class TransactionBehaviour<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher)
{
    protected async Task<TResponse> HandleCore(Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        var span = SentrySdk.GetSpan()?
            .StartChild("transaction", "Database transaction pipeline");

        try
        {
            await unitOfWork.BeginTransactionAsync();
            var response = await next();

            await eventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, cancellationToken);

            await unitOfWork.CommitTransactionAsync();
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
        finally
        {
            span?.Finish();
        }
    }
}

public sealed class CommandTransactionBehaviour<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher)
    : TransactionBehaviour<TCommand, TResponse>(unitOfWork, eventDispatcher),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(() => next(), cancellationToken);
}

public sealed class QueryTransactionBehaviour<TQuery, TResponse>(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher)
    : TransactionBehaviour<TQuery, TResponse>(unitOfWork, eventDispatcher),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(() => next(), cancellationToken);
}
