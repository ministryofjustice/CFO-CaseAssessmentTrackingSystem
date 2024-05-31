using Cfo.Cats.Application.Features.KeyValues.Caching;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Import;

public class ImportKeyValuesCommand : ICacheInvalidatorRequest<Result>
{
    public ImportKeyValuesCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }

    public string FileName { get; set; }
    public byte[] Data { get; set; }
    public string CacheKey { get; }
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
}