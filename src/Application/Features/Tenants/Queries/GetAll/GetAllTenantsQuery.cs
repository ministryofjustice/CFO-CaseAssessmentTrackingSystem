using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Tenants.Queries.GetAll;

[RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsRead)]
public class GetAllTenantsQuery : ICacheableRequest<IEnumerable<TenantDto>>
{
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public MemoryCacheEntryOptions? Options => TenantCacheKey.MemoryCacheEntryOptions;
}