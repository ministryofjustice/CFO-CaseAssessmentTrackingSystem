using Cfo.Cats.Application.Features.KeyValues.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services;

public class CachingPicklistService(IFusionCache cache, PicklistService picklistService, ILogger<CachingPicklistService> logger) :
    IPicklistService
{
    public IReadOnlyList<KeyValueDto> DataSource
    {
        get
        {
            const string cacheKey = "CachingPicklistService-all";

            var cached = cache.TryGet<IReadOnlyList<KeyValueDto>>(cacheKey);

            if (cached.HasValue)
            {
                logger.LogDebug($"Cache entry found for {cacheKey}");
                return cached.Value;
            }

            logger.LogDebug($"Cache entry not found for {cacheKey}");

            var result = picklistService.DataSource;
            if (result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["picklist"]);
                logger.LogDebug($"Cache entry set for {cacheKey}");
            }

            return result;
        }
    }

    public event Action? OnChange;
    public void Refresh()
    {
        logger.LogDebug("Refresh called, clearing the cache");
        cache.RemoveByTag("picklist");
        OnChange?.Invoke();
    }
}