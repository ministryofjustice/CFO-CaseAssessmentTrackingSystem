using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.Caching;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Locations.Queries.GetAll;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
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