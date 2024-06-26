using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Documents.Commands.Upload;

[RequestAuthorize(Policy = PolicyNames.AllowDocumentUpload)]
public class UploadDocumentCommand : ICacheInvalidatorRequest<Result<Guid>>
{
    public string CacheKey { get; } = string.Empty;

    public CancellationTokenSource? SharedExpiryTokenSource
        => DocumentCacheKey.SharedExpiryTokenSource();
    
}