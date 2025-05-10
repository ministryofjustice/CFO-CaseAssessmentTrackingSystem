using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Locations;

public class CachingLocationService(
    IFusionCache cache,
    ILocationService locationService,
    ILogger<CachingLocationService> logger)
    : ILocationService
{
    public IReadOnlyList<LocationDto> DataSource
    {
        get
        {
            const string cacheKey = "CachingLocationService-all";

            var cached = cache.TryGet<IReadOnlyList<LocationDto>>(cacheKey);

            if (cached.HasValue)
            {
                logger.LogDebug($"Cache entry found for {cacheKey}");
                return cached.Value;
            }

            logger.LogDebug($"Cache entry not found for {cacheKey}");

            var result = locationService.DataSource;
            if (result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["locations"]);
                logger.LogDebug($"Cache entry set for {cacheKey}");
            }

            return result;
        }
    }

    public event Action? OnChange;
    public void Refresh()
    {
        logger.LogDebug("Refresh called, clearing the cache");
        cache.RemoveByTag("locations");
        OnChange?.Invoke();
    }

    public IEnumerable<LocationDto> GetVisibleLocations(string tenantId)
    {
        var results = DataSource.Where(l => l.Tenants.Any(t => t.StartsWith(tenantId)));
        return results;
    }
}