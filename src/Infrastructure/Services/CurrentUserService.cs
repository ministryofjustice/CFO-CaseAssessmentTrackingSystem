using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Cfo.Cats.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public int? UserId => httpContextAccessor.HttpContext?.User.GetUserId();
    public string? UserName => httpContextAccessor.HttpContext?.User.GetUserName();
    public string? TenantId => httpContextAccessor.HttpContext?.User.GetTenantId();
    public string? TenantName => httpContextAccessor.HttpContext?.User.GetTenantName();
}
