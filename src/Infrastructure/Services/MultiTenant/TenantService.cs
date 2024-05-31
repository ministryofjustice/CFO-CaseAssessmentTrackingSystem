using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext context;
    private readonly IFusionCache fusionCache;
    private readonly IMapper mapper;

    public TenantService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory,
        IMapper mapper
    )
    {
        var scope = scopeFactory.CreateScope();
        context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        this.fusionCache = fusionCache;
        this.mapper = mapper;
    }

    public event Action? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();

    public void Initialize()
    {
        DataSource =
            fusionCache.GetOrSet(
                TenantCacheKey.TenantsCacheKey,
                _ =>
                    context
                        .Tenants.OrderBy(x => x.Name)
                        .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                        .ToList()
            ) ?? new List<TenantDto>();
    }

    public void Refresh()
    {
        fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        DataSource = fusionCache.GetOrSet(
            TenantCacheKey.TenantsCacheKey,
            _ =>
                context
                    .Tenants.OrderBy(x => x.Name)
                    .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                    .ToList()
        );
        OnChange?.Invoke();
    }
}
