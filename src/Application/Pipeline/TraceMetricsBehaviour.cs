namespace Cfo.Cats.Application.Pipeline;

public class TraceMetricsBehaviour<TRequest, TResponse>(ICurrentUserService currentUserService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).FullName!.Split(".").Last();
        var transaction = SentrySdk.StartTransaction(requestName, "mediator.handle");

        SentrySdk.ConfigureScope(scope =>
        {
            scope.SetTag("tenant_id", currentUserService.TenantId ?? "none");
            scope.SetTag("tenant_name", currentUserService.TenantName ?? "none");
            scope.Transaction = transaction;
        });

        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            transaction.Finish(SpanStatus.InternalError);
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            if(transaction.IsFinished == false)
            {
                transaction.Finish();
            }
            SentrySdk.ConfigureScope(scope => scope.Transaction = null);
        }
    }
}
