using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class KeyValuesWithPaginationQuery : KeyValueAdvancedFilter, ICacheableRequest<PaginatedData<KeyValueDto>>
{
    [JsonIgnore]
    public KeyValueAdvancedSpecification Specification => new(this);

    [JsonIgnore]
    public string CacheKey => $"{nameof(KeyValuesWithPaginationQuery)},{this}";

    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Picklist:{Picklist},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}