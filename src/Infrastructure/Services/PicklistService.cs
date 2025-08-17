using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Infrastructure.Services.Locations;

namespace Cfo.Cats.Infrastructure.Services;

public class PicklistService(IServiceScopeFactory scopeFactory, ILogger<PicklistService> logger, IMapper mapper) 
    : IPicklistService
{
    public event Action? OnChange;

    public IReadOnlyList<KeyValueDto> DataSource
    {
        get
        {
            logger.LogDebug("DataSource called, getting from the database");

            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var data = unitOfWork.DbContext
                .KeyValues
                .OrderBy(e => e.Name)
                .ThenBy(e => e.Value)
                .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
                .ToList();

            return data.AsReadOnly();
        }
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of PicklistService called, ignored as this is the none caching service");
        OnChange?.Invoke();
    }
}