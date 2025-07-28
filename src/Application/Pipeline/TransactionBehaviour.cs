namespace Cfo.Cats.Application.Pipeline;

public class TransactionBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork, IDomainEventDispatcher eventDispatcher) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
   
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;

        await unitOfWork.BeginTransactionAsync();

        try
        {
            response = await next(cancellationToken);

            
            //await unitOfWork.SaveChangesAsync(cancellationToken);
            await eventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, cancellationToken);

            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return response;
    }
    
}
