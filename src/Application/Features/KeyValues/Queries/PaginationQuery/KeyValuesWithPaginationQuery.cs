using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;

[RequestAuthorize(Policy = PolicyNames.AuthorizedUser)]
public class KeyValuesWithPaginationQuery : KeyValueAdvancedFilter, ICacheableRequest<PaginatedData<KeyValueDto>>
{
    public KeyValueAdvancedSpecification Specification => new(this);

    public string CacheKey => $"{nameof(KeyValuesWithPaginationQuery)},{this}";
    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Picklist:{Picklist},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}