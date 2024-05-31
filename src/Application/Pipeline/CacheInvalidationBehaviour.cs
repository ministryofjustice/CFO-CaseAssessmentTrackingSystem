using Cfo.Cats.Application.Common.Interfaces.Caching;

namespace Cfo.Cats.Application.Pipeline;

public class CacheInvalidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorRequest<TResponse>
{
    private readonly IAppCache cache;
    private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger;

    public CacheInvalidationBehaviour(
        IAppCache cache,
        ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
    )
    {
        this.cache = cache;
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        logger.LogTrace("{Name} cache expire with {@Request}", nameof(request), request);
        var response = await next().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            cache.Remove(request.CacheKey);
        }

        request.SharedExpiryTokenSource?.Cancel();
        return response;
    }
}
