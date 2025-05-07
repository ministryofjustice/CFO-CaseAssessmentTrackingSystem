using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.MultiTenant;

public class TenantService(IFusionCache cache, IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<TenantService> logger) 
    : ITenantService
{
    private bool _initialized;
    private readonly object _initLock = new();

    public event Action? OnChange;
    public IReadOnlyList<TenantDto> DataSource => 
        cache.TryGet<IReadOnlyList<TenantDto>>(TenantCacheKey.TenantsCacheKey) is { HasValue: true } result
            ? result.Value
            : [];


    public void Initialize()
    {
        if(_initialized)
        {
            logger.LogInformation("TenantDto cache service is already initialized");
            return;
        }

        lock(_initLock)
        {
            if(_initialized)
            {
                logger.LogInformation("TenantDto cache service is already initialized (after lock)");
                return;
            }
            LoadCache();
            _initialized = true;
        }
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of TenantDto cache called");
        LoadCache();
        OnChange?.Invoke();
    }

    public IEnumerable<TenantDto> GetVisibleTenants(string tenantId) => 
        DataSource.Where(t => t.Id.StartsWith(tenantId));

    private void LoadCache()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
                        .Tenants.OrderBy(x => x.Name)
                        .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                        .ToList();

        cache.Set(TenantCacheKey.TenantsCacheKey, data);

        logger.LogInformation($"{data.Count} TenantDto elements added to cache");
    }
}
