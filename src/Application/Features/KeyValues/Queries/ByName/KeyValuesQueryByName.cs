using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.ByName;

public class KeyValuesQueryByName : ICacheableRequest<IEnumerable<KeyValueDto>>
{
    public KeyValuesQueryByName(Picklist name)
    {
        Name = name;
    }

    public Picklist Name { get; set; }

    public string CacheKey => KeyValueCacheKey.GetCacheKey(Name.ToString());

    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
}