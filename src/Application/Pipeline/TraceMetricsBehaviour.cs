using System.Runtime.CompilerServices;

namespace Cfo.Cats.Application.Pipeline;

public class TraceMetricsBehaviour<TRequest, TResponse>(ICurrentUserService currentUserService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).FullName!.Split(".").Last();
        var transaction = SentrySdk.StartTransaction(requestName, "mediator.handle");

        transaction.SetTag("tenant_id", currentUserService.TenantId ?? "none");
        transaction.SetTag("tenant_name", currentUserService.TenantName ?? "none");
        
        SentrySdk.ConfigureScope(scope =>
        {
            scope.Transaction = transaction;
            scope.User = new SentryUser()
            {
                Id = currentUserService.UserId ?? "none",
                Username = currentUserService.UserName ?? "none",
            };
        });

        try
        {
            return await next(cancellationToken);
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
