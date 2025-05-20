using Cfo.Cats.Application.Features.Delius.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Delius;

public class CachingDeliusService(IFusionCache cache, IDeliusService deliusService)
    : IDeliusService
{
    public async Task<Result<OffenceDto>> GetOffencesAsync(string crn)
    {
        string cacheKey = $"CachingDeliusService-Offences-{crn}";

        var cached = await cache.TryGetAsync<Result<OffenceDto>>(cacheKey);

        if (cached.HasValue)
        {
            return cached.Value;
        }

        var result = await deliusService.GetOffencesAsync(crn);

        if (result.Succeeded)
        {
            // we only cache successful results
            await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["dms"]);
        }

        return result;
    }
}