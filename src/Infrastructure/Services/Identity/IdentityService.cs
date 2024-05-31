using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Extensions;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService authorizationService;
    private readonly IAppCache cache;
    private readonly IStringLocalizer<IdentityService> localizer;
    private readonly IMapper mapper;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> userManager;

    public IdentityService(
        IServiceScopeFactory scopeFactory,
        IAppCache cache,
        IMapper mapper,
        IStringLocalizer<IdentityService> localizer
    )
    {
        var scope = scopeFactory.CreateScope();
        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<
            IUserClaimsPrincipalFactory<ApplicationUser>
        >();
        authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        this.cache = cache;
        this.mapper = mapper;
        this.localizer = localizer;
    }

    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);

    private LazyCacheEntryOptions Options =>
        new LazyCacheEntryOptions().SetAbsoluteExpiration(
            RefreshInterval,
            ExpirationMode.LazyExpiration
        );

    public async Task<string?> GetUserNameAsync(
        int userId,
        CancellationToken cancellation = default
    )
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await cache.GetOrAddAsync(
            key,
            async () => await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId),
            Options
        );
        return user?.UserName;
    }

    public string GetUserName(int userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = cache.GetOrAdd(
            key,
            () => userManager.Users.SingleOrDefault(u => u.Id == userId),
            Options
        );
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(
        int userId,
        string role,
        CancellationToken cancellation = default
    )
    {
        var user =
            await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation)
            ?? throw new NotFoundException(localizer["User Not Found."]);
        return await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(
        int userId,
        string policyName,
        CancellationToken cancellation = default
    )
    {
        var user =
            await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation)
            ?? throw new NotFoundException(localizer["User Not Found."]);
        var principal = await userClaimsPrincipalFactory.CreateAsync(user);
        var result = await authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(
        int userId,
        CancellationToken cancellation = default
    )
    {
        var user =
            await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation)
            ?? throw new NotFoundException(localizer["User Not Found."]);
        var result = await userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<IDictionary<string, string?>> FetchUsers(
        string roleName,
        CancellationToken cancellation = default
    )
    {
        var result = await userManager
            .Users.Where(x => x.UserRoles.Any(y => y.Role.Name == roleName))
            .Include(x => x.UserRoles)
            .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }

    public async Task UpdateLiveStatus(
        int userId,
        bool isLive,
        CancellationToken cancellation = default
    )
    {
        var user = await userManager.Users.FirstOrDefaultAsync(
            x => x.Id == userId && x.IsLive != isLive,
            cancellationToken: cancellation
        );
        if (user is not null)
        {
            user.IsLive = isLive;
            var result = await userManager.UpdateAsync(user);
        }
    }

    public async Task<ApplicationUserDto> GetApplicationUserDto(
        string userName,
        CancellationToken cancellation = default
    )
    {
        var key = $"GetApplicationUserDto:{userName}";
        var result = await cache.GetOrAddAsync(
            key,
            async () =>
                await userManager
                    .Users.Where(x => x.UserName == userName)
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellation)
                ?? new ApplicationUserDto() { UserName = "Anonymous" },
            Options
        );
        return result;
    }

    public async Task<List<ApplicationUserDto>?> GetUsers(
        string? tenantId,
        CancellationToken cancellation = default
    )
    {
        var key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        Func<string?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId =
            async (tenantId, token) =>
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return await userManager
                        .Users.Include(x => x.UserRoles)
                        .ThenInclude(x => x.Role)
                        .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken: token);
                }

                return await userManager
                    .Users.Where(x => x.TenantId == tenantId)
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken: token);
            };
        var result = await cache.GetOrAddAsync(
            key,
            () => getUsersByTenantId(tenantId, cancellation),
            Options
        );
        return result;
    }
}
