using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Delete;

[RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsWrite)]
public class DeleteKeyValueCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteKeyValueCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; }
    public string[] CacheKeys => [ KeyValueCacheKey.GetAllCacheKey ];
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
}