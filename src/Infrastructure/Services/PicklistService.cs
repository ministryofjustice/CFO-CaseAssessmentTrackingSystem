using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services;

public class PicklistService(IFusionCache cache,
        IServiceScopeFactory scopeFactory,
        ILogger<PicklistService> logger,
        IMapper mapper) : IPicklistService
{

    private bool _initialized;
    private readonly object _initLock = new();
    
    public event Action? OnChange;
    public IReadOnlyList<KeyValueDto> DataSource => 
        cache.TryGet<IReadOnlyList<KeyValueDto>>(KeyValueCacheKey.PicklistCacheKey) is { HasValue: true } result
                ? result.Value : [];

    public void Initialize()
    {
        if(_initialized)
        {
            logger.LogInformation("KeyValueDto cache service is already initialized");
            return;
        }

        lock(_initLock)
        {
            if(_initialized)
            {
                logger.LogInformation("KeyValueDto cache service is already initialized (after lock)");
                return;
            }
            LoadCache();
            _initialized = true;
        }
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of KeyValueDto cache called");
        LoadCache();
        OnChange?.Invoke();
    }

    private void LoadCache()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
            .KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
            .ToList();

        cache.Set(KeyValueCacheKey.PicklistCacheKey, data);

        logger.LogInformation($"{data.Count} KeyValueDto elements added to cache");
    }
}
