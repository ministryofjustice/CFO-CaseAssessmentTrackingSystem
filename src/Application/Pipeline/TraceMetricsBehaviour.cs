namespace Cfo.Cats.Application.Pipeline;

public abstract class TraceMetricsBehaviour<TRequest, TResponse>(ICurrentUserService currentUserService)
{
    protected async Task<TResponse> HandleCore(Func<Task<TResponse>> next)
    {
        var requestName = typeof(TRequest).FullName!.Split(".").Last();
        var transaction = SentrySdk.StartTransaction(requestName, "mediator.handle");

        transaction.SetTag("tenant_id", currentUserService.TenantId ?? "none");
        transaction.SetTag("tenant_name", currentUserService.TenantName ?? "none");

        SentrySdk.ConfigureScope(scope =>
        {
            scope.Transaction = transaction;
            scope.User = new SentryUser
            {
                Id = currentUserService.UserId ?? "none",
                Username = currentUserService.UserName ?? "none",
            };
        });

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            transaction.Finish(SpanStatus.InternalError);
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            if (transaction.IsFinished == false)
            {
                transaction.Finish();
            }

            SentrySdk.ConfigureScope(scope => scope.Transaction = null);
        }
    }
}

public sealed class CommandTraceMetricsBehaviour<TCommand, TResponse>(ICurrentUserService currentUserService)
    : TraceMetricsBehaviour<TCommand, TResponse>(currentUserService),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(() => next());
}

public sealed class QueryTraceMetricsBehaviour<TQuery, TResponse>(ICurrentUserService currentUserService)
    : TraceMetricsBehaviour<TQuery, TResponse>(currentUserService),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(() => next());
}
