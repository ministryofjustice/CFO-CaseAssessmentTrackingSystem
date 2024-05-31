using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.Caching;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Locations.Queries.GetAll;

public class GetAllLocationsQuery : ICacheableRequest<Result<LocationDto[]>>
{
    public required UserProfile UserProfile { get; set; }

    public string CacheKey => LocationsCacheKey.GetCacheKey("{this}");

    public MemoryCacheEntryOptions? Options
        => LocationsCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"GetAllLocationsQuery,{UserProfile.TenantId}";
    }
}