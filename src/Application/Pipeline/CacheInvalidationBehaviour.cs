using Cfo.Cats.Application.Common.Interfaces.Caching;

namespace Cfo.Cats.Application.Pipeline;

public sealed class CacheInvalidationBehaviour<TCommand, TResponse>(
    IAppCache cache,
    ILogger<CacheInvalidationBehaviour<TCommand, TResponse>> logger)
    : ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (command is not ICacheInvalidatorRequest<TResponse> cacheInvalidatorRequest)
        {
            return response;
        }

        logger.LogTrace("{Name} cache expire with {@Request}", nameof(command), command);

        foreach (var key in cacheInvalidatorRequest.CacheKeys)
        {
            cache.Remove(key);
        }

        cacheInvalidatorRequest.SharedExpiryTokenSource?.Cancel();
        return response;
    }
}
