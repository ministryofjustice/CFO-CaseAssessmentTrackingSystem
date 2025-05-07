using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Contracts;

public class ContractService(IServiceScopeFactory scopeFactory, IFusionCache cache, IMapper mapper, ILogger<ContractService> logger) 
    : IContractService
{

    private const string CacheKey = "contracts-all";
    
    private bool _initialized;
    private readonly object _initLock = new();
    
    public IReadOnlyList<ContractDto> DataSource => 
        cache.TryGet<IReadOnlyList<ContractDto>>(CacheKey) is { HasValue: true } result
            ? result.Value
            : [];
    
    
    public event Action? OnChange;
    
    public void Initialize()
    {
        if(_initialized)
        {
            logger.LogInformation("ContractDto cache service is already initialized");
            return;
        }

        lock(_initLock)
        {
            if(_initialized)
            {
                logger.LogInformation("ContractDto cache service is already initialized (after lock)");
                return;
            }

            LoadCache();
            _initialized = true;
        }
    }

    public void Refresh()
    {
        logger.LogInformation("Refresh of ContractDto cache called");
        LoadCache();
        OnChange?.Invoke();
    }

    public IEnumerable<ContractDto> GetVisibleContracts(string tenantId)
        => DataSource.Where(c => c.TenantId.StartsWith(tenantId));

    private void LoadCache()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
                .Contracts
                .AsNoTracking()
                .OrderBy(x => x.Description)
                .ProjectTo<ContractDto>(mapper.ConfigurationProvider)
                .ToList();
        
        cache.Set(CacheKey, data);

        logger.LogInformation($"{data.Count} ContractDto elements added to cache");
    }
}