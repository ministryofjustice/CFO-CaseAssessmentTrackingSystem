using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Infrastructure.Services.Contracts;

public class ContractService(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<ContractService> logger) 
    : IContractService
{
    public IReadOnlyList<ContractDto> DataSource
    {
        get
        {
            logger.LogDebug("DataSource called, getting from the database");

            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var data = unitOfWork.DbContext
                .Contracts
                .OrderBy(e => e.Description)
                .ProjectTo<ContractDto>(mapper.ConfigurationProvider)
                .ToList();

            return data.AsReadOnly();
        }
    }
    public event Action? OnChange;

    public void Refresh()
    {
        logger.LogInformation("Refresh of ContractService called, ignored as this is the none caching service");
        OnChange?.Invoke();
    }

    public IEnumerable<ContractDto> GetVisibleContracts(string tenantId)
    {
        logger.LogDebug("GetVisibleContracts called, getting from the database");

        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = unitOfWork.DbContext
            .Contracts
            .Where(c => c.Tenant!.Id.StartsWith(tenantId))
            .OrderBy(e => e.Description)
            .ProjectTo<ContractDto>(mapper.ConfigurationProvider)
            .ToList();

        return data.AsReadOnly();
    }
}