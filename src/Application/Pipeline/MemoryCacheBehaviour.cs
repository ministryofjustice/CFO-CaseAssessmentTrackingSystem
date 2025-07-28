using Cfo.Cats.Application.Common.Interfaces.Caching;

namespace Cfo.Cats.Application.Pipeline;

public class MemoryCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    private readonly IAppCache cache;
    private readonly ILogger<MemoryCacheBehaviour<TRequest, TResponse>> logger;

    public MemoryCacheBehaviour(
        IAppCache cache,
        ILogger<MemoryCacheBehaviour<TRequest, TResponse>> logger
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
        logger.LogTrace("{Name} is caching with {@Request}", nameof(request), request);
        var response = await cache
            .GetOrAddAsync(request.CacheKey, async () => await next(cancellationToken), request.Options)
            .ConfigureAwait(false);

        return response;
    }
}
