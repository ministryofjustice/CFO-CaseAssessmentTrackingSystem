using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Infrastructure.Services.Locations;

namespace Cfo.Cats.Infrastructure.Services.MultiTenant;

public class TenantService(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<TenantService> logger) 
    : ITenantService
{
    public IReadOnlyList<TenantDto> DataSource 
    {
        get
        {
            logger.LogDebug("DataSource called, getting from the database");

            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var data = unitOfWork.DbContext
                .Tenants
                .OrderBy(e => e.Name)
                .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                .ToList();

            return data.AsReadOnly();
        }
    }
    public event Action? OnChange;
    public void Refresh()
    {
        logger.LogInformation("Refresh of TenantService called, ignored as this is the none caching service");
        OnChange?.Invoke();
    }

    public IEnumerable<TenantDto> GetVisibleTenants(string tenantId)
    {
        logger.LogDebug("GetVisibleTenants called, getting from the database");

        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
            .Tenants
            .Where(l => l.Id.StartsWith(tenantId))
            .OrderBy(e => e.Name)
            .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
            .ToList();

        return data.AsReadOnly();

    }
}