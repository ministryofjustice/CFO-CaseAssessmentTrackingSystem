using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Caching;

namespace Cfo.Cats.Application.Features.Tenants.Commands.Delete;

[RequestAuthorize(Roles = "Admin")]
public class DeleteTenantCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteTenantCommand(string[] id)
    {
        Id = id;
    }

    public string[] Id { get; }
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource =>
        TenantCacheKey.SharedExpiryTokenSource();
}