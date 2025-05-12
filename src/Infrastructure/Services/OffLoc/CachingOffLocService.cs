using Cfo.Cats.Application.Features.Offloc.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.OffLoc;

public class CachingOffLocService(IFusionCache cache, IOfflocService offlocService)
    : IOfflocService
{
    public async Task<Result<PersonalDetailsDto>> GetPersonalDetailsAsync(string nomisNumber)
    {
        string cacheKey = $"CachingOffLockService-PersonalDetails-{nomisNumber}";

        var cached = await cache.TryGetAsync<Result<PersonalDetailsDto>>(cacheKey);

        if (cached.HasValue)
        {
            return cached.Value;
        }

        var result = await offlocService.GetPersonalDetailsAsync(nomisNumber);

        if (result.Succeeded)
        {
            // we only cache successful results
            await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(20));
        }

        return result;
    }
}