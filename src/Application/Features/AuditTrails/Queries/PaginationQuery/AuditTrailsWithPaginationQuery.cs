using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.AuditTrails.Caching;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;
using Cfo.Cats.Application.Features.AuditTrails.Specifications;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries.PaginationQuery;

[RequestAuthorize(Roles = "Admin")]
public class AuditTrailsWithPaginationQuery
    : AuditTrailAdvancedFilter,
        ICacheableRequest<PaginatedData<AuditTrailDto>>
{
    public AuditTrailAdvancedSpecification Specification => new(this);

    public string CacheKey => AuditTrailsCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => AuditTrailsCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Listview:{ListView},AuditType:{AuditType},Search:{Keyword},Sort:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}