using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class UserService(IFusionCache cache, IMapper mapper, IServiceScopeFactory scopeFactory, ILogger<UserService> logger) 
    : IUserService
{
    private const string Cachekey = "ALL-ApplicationUserDto";
    private bool _initialized;
    private readonly object _initLock = new();
    
    public IReadOnlyList<ApplicationUserDto> DataSource => 
        cache.TryGet<IReadOnlyList<ApplicationUserDto>>(Cachekey) is { HasValue: true } result
            ? result.Value
            : [];

    public event Action? OnChange;

    public string? GetDisplayName(string userId)
    {
        return DataSource.FirstOrDefault(u => u.Id == userId)?.DisplayName;
    }

    public void Initialize()
    {
        if(_initialized)
        {
            logger.LogInformation("ApplicationUserDto cache service is already initialized");
            return;
        }

        lock(_initLock)
        {
            if(_initialized)
            {
                logger.LogInformation("ApplicationUserDto cache service is already initialized (after lock)");
                return;
            }

            LoadCache();
            _initialized = true;
        }
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of ApplicationUserDto cache called");
        LoadCache();
        OnChange?.Invoke();
    }

    private void LoadCache()
    {
        var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var data = userManager
                .Users.Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.UserName)
                .ToList();

        cache.Set(Cachekey, data);

        logger.LogInformation($"{data.Count} ApplicationUserDto elements added to cache");
    }
}
