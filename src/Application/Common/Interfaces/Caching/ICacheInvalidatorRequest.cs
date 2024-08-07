﻿namespace Cfo.Cats.Application.Common.Interfaces.Caching;

public interface ICacheInvalidatorRequest<TResponse> : IRequest<TResponse>
{
    string[] CacheKeys { get; }

    CancellationTokenSource? SharedExpiryTokenSource { get; }
}
