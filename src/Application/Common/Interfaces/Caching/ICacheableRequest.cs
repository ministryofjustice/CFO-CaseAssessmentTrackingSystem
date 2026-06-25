namespace Cfo.Cats.Application.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }
    MemoryCacheEntryOptions? Options { get; }
}
