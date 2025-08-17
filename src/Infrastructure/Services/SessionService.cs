using Cfo.Cats.Application.Common.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services;

public class SessionService(IMemoryCache cache, IConfiguration configuration) : ISessionService
{
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(configuration.GetValue<int>("IdleTimeOutMinutes"));

    public void StartSession(string? userId)
    {
        if (string.IsNullOrEmpty(userId) == false)
        {
            cache.Set($"LastActivity_{userId}", DateTime.UtcNow, _timeout);
        }
    }
    
    public bool IsSessionValid(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        var lastActivity = cache.Get<DateTime?>($"LastActivity_{userId}");
        
        if (lastActivity.HasValue == false)
        {
            // No session found
            return false; 
        }

        if (DateTime.UtcNow - lastActivity.Value > _timeout)
        {
            // The session has passed
            InvalidateSession(userId);
            return false;
        }

        return true;
    }
    
    public void UpdateActivity(string? userId)
    {
        if (string.IsNullOrEmpty(userId) == false)
        {
            cache.Set($"LastActivity_{userId}", DateTime.UtcNow, _timeout);
        }
    }

    public void InvalidateSession(string? userId)
    {
        if (string.IsNullOrEmpty(userId) == false)
        {
            cache.Remove($"LastActivity_{userId}");
        }
    }

}
