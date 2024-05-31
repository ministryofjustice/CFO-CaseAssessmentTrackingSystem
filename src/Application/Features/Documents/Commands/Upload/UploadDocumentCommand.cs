using Cfo.Cats.Application.Features.Documents.Caching;

namespace Cfo.Cats.Application.Features.Documents.Commands.Upload;

public class UploadDocumentCommand : ICacheInvalidatorRequest<Result<Guid>>
{
    public string CacheKey { get; } = string.Empty;

    public CancellationTokenSource? SharedExpiryTokenSource
        => DocumentCacheKey.SharedExpiryTokenSource();
    
}