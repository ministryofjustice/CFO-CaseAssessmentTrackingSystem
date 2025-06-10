using System.Diagnostics;

namespace Cfo.Cats.Application.Pipeline;

/// <summary>
///     This class is a behavior pipeline in MediatR. It is used to monitor performance
///     and log warnings if a request takes longer to execute than a specified threshold.
/// </summary>
/// <typeparam name="TRequest">Type of the Request</typeparam>
/// <typeparam name="TResponse">Type of the Response</typeparam>
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService currentUserService;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> logger;

    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService
    )
    {
        this.logger = logger;
        this.currentUserService = currentUserService;
    }

    /// <summary>
    ///     Logs mediator calls to sentry
    /// </summary>
    /// <param name="request">The incoming request</param>
    /// <param name="next">The delegate for the next action in the pipeline process.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the next delegate</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        
        var requestName = typeof(TRequest).FullName!;

        var transaction = SentrySdk.StartTransaction(requestName, "mediator-call");
        try
        {
            var response = await next(cancellationToken)
                .ConfigureAwait(false);
            transaction.Finish(SpanStatus.Ok);
            return response;
        }
        catch (Exception ex)
        {
            transaction.Finish(ex);
            throw;
        }
    }
}