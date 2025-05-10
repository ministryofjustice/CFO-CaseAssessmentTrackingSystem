using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Contracts;

public class CachingContractService(IFusionCache cache, IContractService contractService, ILogger<CachingContractService> logger) : IContractService
{
    public IReadOnlyList<ContractDto> DataSource
    {
        get
        {
            const string cacheKey = "CachingContractService-all";

            var cached = cache.TryGet<IReadOnlyList<ContractDto>>(cacheKey);

            if (cached.HasValue)
            {
                logger.LogDebug($"Cache entry found for {cacheKey}");
                return cached.Value;
            }

            logger.LogDebug($"Cache entry not found for {cacheKey}");

            var result = contractService.DataSource;
            if (result is { Count: > 0 })
            {
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["contracts"]);
                logger.LogDebug($"Cache entry set for {cacheKey}");
            }

            return result;
        }
    }
    public event Action? OnChange;
    public void Refresh()
    {
        logger.LogDebug("Refresh called, clearing the cache");
        cache.RemoveByTag("contracts");
        OnChange?.Invoke();
    }

    public IEnumerable<ContractDto> GetVisibleContracts(string tenantId)
    {
        var results = DataSource.Where(c => c.TenantId.StartsWith(tenantId));
        return results;
    }
}