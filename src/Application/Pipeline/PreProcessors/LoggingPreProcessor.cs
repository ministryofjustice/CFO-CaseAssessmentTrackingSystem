namespace Cfo.Cats.Application.Pipeline.PreProcessors;

public class LoggingPreProcessor<TRequest>(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = nameof(TRequest);
        var userName = currentUserService.UserName;
        logger.LogTrace(
            "Request: {Name} with {@Request} by {@UserName}",
            requestName,
            request,
            userName
        );
        return Task.CompletedTask;
    }
}
