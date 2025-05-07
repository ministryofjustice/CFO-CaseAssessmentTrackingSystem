using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.Caching;
using Cfo.Cats.Application.Features.Locations.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Locations;

public class LocationService(IServiceScopeFactory scopeFactory, IFusionCache cache, IMapper mapper, ILogger<LocationService> logger) 
    : ILocationService
{
    
    private bool _initialized;
    private readonly object _initLock = new();

    public IReadOnlyList<LocationDto> DataSource =>  cache.TryGet<IReadOnlyList<LocationDto>>(LocationsCacheKey.GetCacheKey("all")) is { HasValue: true } result
            ? result.Value
            : [];

    public event Action? OnChange;

    public IEnumerable<LocationDto> GetVisibleLocations(string tenantId) => DataSource.Where(c => c.Tenants.Any(t => t.StartsWith(tenantId)));

    public void Initialize()
    {   
        if(_initialized)
        {
            logger.LogInformation("LocationDto cache service is already initialized");
            return;
        }

        lock(_initLock)
        {
            if(_initialized)
            {
                logger.LogInformation("LocationDto cache service is already initialized (after lock)");
                return;
            }
            LoadCache();
            _initialized = true;
        }
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of LocationDto cache called");
        LoadCache();
        OnChange?.Invoke();
    }

    private void LoadCache()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
                    .Locations.OrderBy(x => x.Name)
                    .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
                    .ToList();

        cache.Set(LocationsCacheKey.GetCacheKey("all"), data);

        logger.LogInformation($"{data.Count} LocationDto elements added to cache");
    }
}
