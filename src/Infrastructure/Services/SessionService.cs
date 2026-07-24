using Cfo.Cats.Application.Common.Security;
using Microsoft.Extensions.Configuration;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services;

public class SessionService(IFusionCache cache, IApplicationSettings settings) : ISessionService
{
    private readonly TimeSpan _timeout =
        TimeSpan.FromMinutes(settings.IdleTimeOutMinutes);

    private static string Key(string userId) => $"LastActivity_{userId}";

    private FusionCacheEntryOptions IdleOptions() =>
        new FusionCacheEntryOptions
        {
            Duration = _timeout,
            IsFailSafeEnabled = false
        };

    public Task StartSessionAsync(string? userId, CancellationToken cancellationToken = default)
        => SetSafeAsync(userId, cancellationToken);

    public Task UpdateActivityAsync(string? userId, CancellationToken cancellationToken = default)
        => SetSafeAsync(userId, cancellationToken);

    public async Task InvalidateSessionAsync(string? userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }
        await cache.RemoveAsync(Key(userId), token: cancellationToken);
    }

    public async Task<bool> IsSessionValidAsync(string? userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }
        return (await cache.TryGetAsync<DateTime>(Key(userId), token: cancellationToken)).HasValue;
    }

    public async Task<TimeSpan?> GetRemainingSessionTimeAsync(string? userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var entry = await cache.TryGetAsync<DateTime>(Key(userId), token: cancellationToken);
        if (entry.HasValue == false)
        {
            return null;
        }

        var lastActivityUtc = entry.Value;
        var elapsed = DateTime.UtcNow - lastActivityUtc;
        var remaining = _timeout - elapsed;

        if (remaining <= TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        if (remaining > _timeout)
        {
            return _timeout;
        }

        return remaining;
    }

    private async Task SetSafeAsync(string? userId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(userId))
        {
            await cache.SetAsync(Key(userId), DateTime.UtcNow, IdleOptions(), token: cancellationToken);
        }
    }
}
