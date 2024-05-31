using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.GetAll;

public class GetAllKeyValuesQuery : ICacheableRequest<IEnumerable<KeyValueDto>>
{
    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
}