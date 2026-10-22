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

    public IReadOnlyList<ApplicationUserSummaryDto> Summaries
    {
        get
        {
            const string cacheKey = "CachingUserService-summaries";

            var cached = cache.TryGet<IReadOnlyList<ApplicationUserSummaryDto>>(cacheKey);

            if (cached.HasValue)
            {
                logger.LogDebug($"Cache entry found for {cacheKey}");
                return cached.Value;
            }

            logger.LogDebug($"Cache entry not found for {cacheKey}");

            var result = userService.Summaries;
            if (result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["users"]);
                logger.LogDebug($"Cache entry set for {cacheKey}");
            }

            return result;
        }
    }

    public event Action? OnChange;

    private IReadOnlyDictionary<string, string> DisplayNames
    {
        get
        {
            const string cacheKey = "CachingUserService-displaynames";

            var cached = cache.TryGet<IReadOnlyDictionary<string, string>>(cacheKey);

            if (cached.HasValue)
            {
                return cached.Value;
            }

            var result = Summaries.ToDictionary(u => u.Id, u => u.DisplayName);
            if (result.Count > 0)
            {
                cache.Set(cacheKey, (IReadOnlyDictionary<string, string>)result, TimeSpan.FromMinutes(20), tags: ["users"]);
            }

            return result;
        }
    }

    public string? GetDisplayName(string? userId)
    {
        if (userId is null)
        {
            return null;
        }

        return DisplayNames.TryGetValue(userId, out var name) ? name : null;
    }

    public void Refresh()
    {
        cache.RemoveByTag("users");
        OnChange?.Invoke();
    }
}
