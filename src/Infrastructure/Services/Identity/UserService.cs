using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private const string Cachekey = "ALL-ApplicationUserDto";
    private readonly IFusionCache fusionCache;
    private readonly IMapper mapper;
    private readonly UserManager<ApplicationUser> userManager;

    public UserService(IFusionCache fusionCache, IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        this.fusionCache = fusionCache;
        this.mapper = mapper;
        var scope = scopeFactory.CreateScope();
        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        DataSource = new List<ApplicationUserDto>();
    }

    public List<ApplicationUserDto> DataSource { get; private set; }

    public event Action? OnChange;

    public string? GetDisplayName(string userId)
    {
        return DataSource.FirstOrDefault(u => u.Id == userId)?.DisplayName;
    }

    public void Initialize()
    {
        DataSource =
            fusionCache.GetOrSet(
                Cachekey,
                _ =>
                    userManager
                        .Users.Include(x => x.UserRoles)
                        .ThenInclude(x => x.Role)
                        .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                        .OrderBy(x => x.UserName)
                        .ToList()
            ) ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }

    public void Refresh()
    {
        fusionCache.Remove(Cachekey);
        DataSource =
            fusionCache.GetOrSet(
                Cachekey,
                _ =>
                    userManager
                        .Users.Include(x => x.UserRoles)
                        .ThenInclude(x => x.Role)
                        .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                        .OrderBy(x => x.UserName)
                        .ToList()
            ) ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }
}
