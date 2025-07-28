namespace Cfo.Cats.Application.Pipeline;

public class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            var requestName = typeof(TRequest).FullName!.Split(".").Last();
            var userName = currentUserService.UserName ?? "Unknown";

            logger.LogDebug(
                "Handling Request: {RequestName} by {UserName}",
                requestName,
                userName
            );
        }

        var response = await next(cancellationToken);
        return response;
    }
}