using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Initiatives;

public class CachingInitiativeService(IFusionCache cache, IInitiativeService initiativeService, ILogger<CachingInitiativeService> logger)
    : IInitiativeService
{
    public IReadOnlyList<InitiativeDto> DataSource
    {
        get
        {
            const string cacheKey = "CachingInitiativeService-all";

            var cached = cache.TryGet<IReadOnlyList<InitiativeDto>>(cacheKey);

            if (cached.HasValue)
            {
                logger.LogDebug("Cache entry found for {CacheKey}", cacheKey);
                return cached.Value;
            }

            logger.LogDebug("Cache entry not found for {CacheKey}", cacheKey);

            var result = initiativeService.DataSource;
            if (result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["initiatives"]);
                logger.LogDebug("Cache entry set for {CacheKey}", cacheKey);
            }

            return result;
        }
    }

    public event Action? OnChange;

    public void Refresh()
    {
        logger.LogDebug("Refresh called, clearing the cache");
        cache.RemoveByTag("initiatives");
        OnChange?.Invoke();
    }

    public IEnumerable<InitiativeDto> GetInitiatives(string tenantId, bool activeOnly = true) =>
        initiativeService.GetInitiatives(tenantId, activeOnly);
}
