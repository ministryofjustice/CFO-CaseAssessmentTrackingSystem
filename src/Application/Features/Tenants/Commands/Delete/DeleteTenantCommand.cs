using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Tenants.Commands.Delete;

[RequestAuthorize(Policy = PolicyNames.SystemFunctionsWrite)]
public class DeleteTenantCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteTenantCommand(string[] id)
    {
        Id = id;
    }

    public string[] Id { get; }
    public string[] CacheKeys => [TenantCacheKey.GetAllCacheKey];
    public CancellationTokenSource? SharedExpiryTokenSource =>
        TenantCacheKey.SharedExpiryTokenSource();
}