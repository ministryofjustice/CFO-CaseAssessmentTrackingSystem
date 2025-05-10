using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.MultiTenant;

public class CachingTenantService(IFusionCache cache, ITenantService tenantService, ILogger<CachingTenantService> logger) 
    : ITenantService
{
    public IReadOnlyList<TenantDto> DataSource 
    {
        get
        {
            const string cacheKey = "CachingTenantService-all";

            var cached = cache.TryGet<IReadOnlyList<TenantDto>>(cacheKey);

            if (cached.HasValue)
            {
                logger.LogDebug($"Cache entry found for {cacheKey}");
                return cached.Value;
            }

            logger.LogDebug($"Cache entry not found for {cacheKey}");

            var result = tenantService.DataSource;
            if (result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["tenants"]);
                logger.LogDebug($"Cache entry set for {cacheKey}");
            }

            return result;
        }
    }
    public event Action? OnChange;
    public void Refresh()
    {
        logger.LogDebug("Refresh called, clearing the cache");
        cache.RemoveByTag("tenants");
        OnChange?.Invoke();
    }

    public IEnumerable<TenantDto> GetVisibleTenants(string tenantId)
    {
        var results = DataSource.Where(l => l.Id.StartsWith(tenantId));
        return results;
    }
}