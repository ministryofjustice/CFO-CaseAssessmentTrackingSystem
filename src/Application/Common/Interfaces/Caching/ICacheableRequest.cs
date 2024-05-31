namespace Cfo.Cats.Application.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get; }
    MemoryCacheEntryOptions? Options { get; }
}
