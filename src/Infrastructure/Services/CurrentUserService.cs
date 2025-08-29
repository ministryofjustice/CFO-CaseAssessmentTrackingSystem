using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Cfo.Cats.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User.GetUserId();
    public string? UserName => httpContextAccessor.HttpContext?.User.GetUserName();
    public string? TenantId => httpContextAccessor.HttpContext?.User.GetTenantId();
    public string? TenantName => httpContextAccessor.HttpContext?.User.GetTenantName();
    public string? DisplayName => httpContextAccessor.HttpContext?.User.GetDisplayName();
}
