using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.Caching;
using Cfo.Cats.Application.Features.Locations.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Locations;

public class LocationService(IServiceScopeFactory scopeFactory, IFusionCache fusionCache, IMapper mapper) : ILocationService
{
    public List<LocationDto> DataSource { get; private set; } = new();

    public event Action? OnChange;

    public IEnumerable<LocationDto> GetVisibleLocations(string tenantId) => DataSource.Where(c => c.Tenants.Any(t => t.StartsWith(tenantId)));

    public void Initialize()
    {   
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        DataSource = fusionCache.GetOrSet(
                LocationsCacheKey.GetCacheKey("all"),
                _ => unitOfWork.DbContext
                    .Locations.OrderBy(x => x.Name)
                    .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
                    .ToList()) ?? [];
    }

    public void Refresh()
    {
        LocationsCacheKey.GetCacheKey("all");
        Initialize();
        OnChange?.Invoke();
    }
}
