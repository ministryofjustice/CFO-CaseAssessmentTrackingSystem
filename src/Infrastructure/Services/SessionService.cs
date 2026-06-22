using Cfo.Cats.Application.Common.Security;
using Microsoft.Extensions.Configuration;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services;

public class SessionService(IFusionCache cache, IConfiguration configuration) : ISessionService
{
    private readonly TimeSpan _timeout =
        TimeSpan.FromMinutes(configuration.GetValue<int>("IdleTimeOutMinutes"));

    private static string Key(string userId) => $"LastActivity_{userId}";

    private FusionCacheEntryOptions IdleOptions() =>
        new FusionCacheEntryOptions
        {
            Duration = _timeout,
            IsFailSafeEnabled = false
        };

    public void StartSession(string? userId)
        => SetSafe(userId);

    public void UpdateActivity(string? userId)
        => SetSafe(userId);

    public void InvalidateSession(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }
        cache.Remove(Key(userId));
    }

    public bool IsSessionValid(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }
        return cache.TryGet<DateTime>(Key(userId)).HasValue;
    }

    public TimeSpan? GetRemainingSessionTime(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var entry = cache.TryGet<DateTime>(Key(userId));
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

    private void SetSafe(string? userId)
    {
        if (!string.IsNullOrWhiteSpace(userId))
        {
            cache.Set(Key(userId), DateTime.UtcNow, IdleOptions());
        }
    }
}
