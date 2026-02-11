using Cfo.Cats.Application.Common.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services;

public class SessionService(IMemoryCache cache, IConfiguration configuration) : ISessionService
{
    private readonly TimeSpan _timeout =
        TimeSpan.FromMinutes(configuration.GetValue<int>("IdleTimeOutMinutes"));

    private static string Key(string userId) => $"LastActivity_{userId}";

    private MemoryCacheEntryOptions IdleOptions() =>
        new() { SlidingExpiration = _timeout };

    public void StartSession(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }
        cache.Set(Key(userId), DateTime.UtcNow, IdleOptions());
    }

    public void UpdateActivity(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }
        cache.Set(Key(userId), DateTime.UtcNow, IdleOptions());
    }

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
        return cache.TryGetValue<DateTime>(Key(userId), out _);
    }

    public TimeSpan? GetRemainingSessionTime(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        if (cache.TryGetValue<DateTime>(Key(userId), out var lastActivityUtc) == false)
        {
            return null; 
        }

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
}
