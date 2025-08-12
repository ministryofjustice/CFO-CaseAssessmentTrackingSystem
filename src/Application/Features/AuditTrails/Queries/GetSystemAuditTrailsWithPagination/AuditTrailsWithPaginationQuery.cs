using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.AuditTrails.Caching;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;
using Cfo.Cats.Application.Features.AuditTrails.Specifications.SystemAuditTrail;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries.GetSystemAuditTrailsWithPagination;

[RequestAuthorize(Policy = SecurityPolicies.ViewAudit)]
public class AuditTrailsWithPaginationQuery
    : SystemAuditTrailAdvancedFilter,
        ICacheableRequest<PaginatedData<AuditTrailDto>>
{
    public SystemAuditTrailAdvancedSpecification Specification => new(this);

    public string CacheKey => SystemAuditTrailsCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => SystemAuditTrailsCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Listview:{ListView},AuditType:{AuditType},Search:{Keyword},Sort:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}