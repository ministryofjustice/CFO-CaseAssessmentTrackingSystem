using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Delete;

[RequestAuthorize(Roles = "Admin, Basic")]
public class DeleteKeyValueCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteKeyValueCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; }
    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
}