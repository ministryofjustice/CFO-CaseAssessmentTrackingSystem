using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Import;

[RequestAuthorize(Roles = "Admin")]
public class ImportKeyValuesCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
}