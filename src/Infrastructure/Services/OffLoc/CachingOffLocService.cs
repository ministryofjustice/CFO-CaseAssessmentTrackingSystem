using Cfo.Cats.Application.Features.Offloc.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.OffLoc;

public class CachingOffLocService(IFusionCache cache, IOfflocService offlocService)
    : IOfflocService
{
    public async Task<Result<SentenceDataDto>> GetSentenceDataAsync(string nomsNumber)
    {
        string cacheKey = $"CachingOffLockService-SentenceDataDto-{nomsNumber}";

        var cached = await cache.TryGetAsync<Result<SentenceDataDto>>(cacheKey);

        if (cached.HasValue)
        {
            return cached.Value;
        }

        var result = await offlocService.GetSentenceDataAsync(nomsNumber);

        if (result.Succeeded)
        {
            await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(20), tags: ["dms"]);
        }

        return result;
    }
}