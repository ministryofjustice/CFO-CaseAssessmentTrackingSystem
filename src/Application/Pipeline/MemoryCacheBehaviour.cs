using Cfo.Cats.Application.Common.Interfaces.Caching;

namespace Cfo.Cats.Application.Pipeline;

public sealed class MemoryCacheBehaviour<TQuery, TResponse>(
    IAppCache cache,
    ILogger<MemoryCacheBehaviour<TQuery, TResponse>> logger)
    : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (query is not ICacheableRequest<TResponse> cacheableRequest)
        {
            return await next();
        }

        logger.LogTrace("{Name} is caching with {@Request}", nameof(query), query);
        var response = await cache
            .GetOrAddAsync(cacheableRequest.CacheKey, async () => await next(), cacheableRequest.Options);

        return response;
    }
}
