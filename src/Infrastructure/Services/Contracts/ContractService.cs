using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Contracts;

public class ContractService(IServiceScopeFactory scopeFactory, IFusionCache cache, IMapper mapper) 
    : IContractService
{

    private const string CacheKey = "contracts-all";
    
    public List<ContractDto> DataSource { get; private set; } = [];
    public event Action? OnChange;
    
    public void Initialize()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        DataSource = cache.GetOrSet(
            CacheKey,
            _ => unitOfWork.DbContext
                .Contracts
                .AsNoTracking()
                .OrderBy(x => x.Description)
                .ProjectTo<ContractDto>(mapper.ConfigurationProvider)
                .ToList());
    }

    public void Refresh()
    {
        cache.Remove(CacheKey);
        Initialize();
        OnChange?.Invoke();
    }

    public IEnumerable<ContractDto> GetVisibleContracts(string tenantId)
        => DataSource.Where(c => c.TenantId.StartsWith(tenantId));
}