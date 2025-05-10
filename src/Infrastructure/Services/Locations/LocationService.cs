using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Infrastructure.Services.Locations;

public class LocationService(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<LocationService> logger) 
    : ILocationService
{
    
    public IReadOnlyList<LocationDto> DataSource
    {
        get
        {
            logger.LogDebug("DataSource called, getting from the database");

            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var data = unitOfWork.DbContext
                .Locations
                .OrderBy(e => e.Name)
                .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
                .ToList();

            return data.AsReadOnly();
        }
    }

    public event Action? OnChange;

    public IEnumerable<LocationDto> GetVisibleLocations(string tenantId)
    {
        logger.LogDebug("GetVisibleLocations called, getting from the database");

        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
            .Locations
            .Where(l => l.Tenants.Any(t => t.Id.StartsWith(tenantId)))
            .OrderBy(e => e.Name)
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToList();

        return data.AsReadOnly();
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of LocationService called, ignored as this is the none caching service");
        OnChange?.Invoke();
    }

 
}