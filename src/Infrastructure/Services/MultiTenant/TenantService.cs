using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IFusionCache fusionCache;
    private readonly IMapper mapper;

    public TenantService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory,
        IMapper mapper
    )
    {
        this.scopeFactory = scopeFactory;
        this.fusionCache = fusionCache;
        this.mapper = mapper;
    }

    public event Action? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();

    public void Initialize()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        DataSource =
            fusionCache.GetOrSet(
                TenantCacheKey.TenantsCacheKey,
                _ =>
                    unitOfWork.DbContext
                        .Tenants.OrderBy(x => x.Name)
                        .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                        .ToList()
            ) ?? [];
    }

    public void Refresh()
    {
        fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        Initialize();
        OnChange?.Invoke();
    }

    public IEnumerable<TenantDto> GetVisibleTenants(string tenantId) => 
        DataSource.Where(t => t.Id.StartsWith(tenantId));
}
