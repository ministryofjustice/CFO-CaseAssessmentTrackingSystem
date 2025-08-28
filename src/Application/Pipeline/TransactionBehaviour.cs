namespace Cfo.Cats.Application.Pipeline;

public class TransactionBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork, IDomainEventDispatcher eventDispatcher) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
   
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var span = SentrySdk.GetSpan()?
            .StartChild("transaction", "Database transaction pipeline");

        try
        {
            await unitOfWork.BeginTransactionAsync();
            var response = await next(cancellationToken);

            //await unitOfWork.SaveChangesAsync(cancellationToken);
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
