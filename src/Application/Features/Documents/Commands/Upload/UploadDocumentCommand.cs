using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.Caching;

namespace Cfo.Cats.Application.Features.Documents.Commands.Upload;

[RequestAuthorize(Roles = "Admin, Basic")]
public class UploadDocumentCommand : ICacheInvalidatorRequest<Result<Guid>>
{
    public string CacheKey { get; } = string.Empty;

    public CancellationTokenSource? SharedExpiryTokenSource
        => DocumentCacheKey.SharedExpiryTokenSource();
    
}