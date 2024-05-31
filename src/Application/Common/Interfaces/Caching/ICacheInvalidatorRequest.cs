namespace Cfo.Cats.Application.Common.Interfaces.Caching;

public interface ICacheInvalidatorRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get; }

    CancellationTokenSource? SharedExpiryTokenSource { get; }
}
