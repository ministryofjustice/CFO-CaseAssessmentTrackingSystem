namespace Cfo.Cats.Application.Common.Interfaces.Caching;

public interface ICacheInvalidatorRequest<TResponse> : ICommand<TResponse>
{
    string[] CacheKeys { get; }

    CancellationTokenSource? SharedExpiryTokenSource { get; }
}
