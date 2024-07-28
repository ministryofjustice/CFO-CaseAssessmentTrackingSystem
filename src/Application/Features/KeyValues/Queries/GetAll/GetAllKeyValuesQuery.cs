using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.GetAll;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class GetAllKeyValuesQuery : ICacheableRequest<IEnumerable<KeyValueDto>>
{
    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
}