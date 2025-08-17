using Cfo.Cats.Application.Features.Identity.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class CachingUserService(IFusionCache cache, IUserService userService, ILogger<CachingUserService> logger) : IUserService
{
    public IReadOnlyList<ApplicationUserDto> DataSource
    {
        get
        {
            const string cacheKey = "CachingUserService-all";          

            var cached = cache.TryGet<IReadOnlyList<ApplicationUserDto>>(cacheKey);

            if(cached.HasValue)
            {
                logger.LogDebug($"Cache entry found for {cacheKey}");
                return cached.Value;
            }

            logger.LogDebug($"Cache entry not found for {cacheKey}");

            var result = userService.DataSource;
            if(result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["users"]);
                logger.LogDebug($"Cache entry set for {cacheKey}");
            }

            return result;
        }
    }

    public event Action? OnChange;

    public string? GetDisplayName(string userId)
    {
        var result = DataSource.FirstOrDefault(r => r.Id == userId)?.DisplayName;
        return result;
    }

    public void Refresh()
    {
        cache.RemoveByTag("users");
        OnChange?.Invoke();
    }
}