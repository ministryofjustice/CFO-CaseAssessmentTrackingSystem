using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Pagination;

[RequestAuthorize(Roles = "Admin, Basic")]
public class TenantsWithPaginationQuery
    : PaginationFilter,
        ICacheableRequest<PaginatedData<TenantDto>>
{
    public TenantsPaginationSpecification Specification => new(this);
    public string CacheKey => TenantCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => TenantCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

#nullable disable warnings