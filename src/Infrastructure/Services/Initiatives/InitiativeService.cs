using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;

namespace Cfo.Cats.Infrastructure.Services.Initiatives;

public class InitiativeService(IServiceScopeFactory scopeFactory, ILogger<InitiativeService> logger)
    : IInitiativeService
{
    public IReadOnlyList<InitiativeDto> DataSource
    {
        get
        {
            logger.LogDebug("DataSource called, getting from the database");

            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var data = unitOfWork.DbContext.Initiatives
                .OrderBy(i => i.Code)
                .Select(InitiativeMappings.ToDto)
                .ToList();

            return data.AsReadOnly();
        }
    }

    public event Action? OnChange;

    public void Refresh()
    {
        logger.LogInformation("Refresh of InitiativeService called, ignored as this is the non-caching service");
        OnChange?.Invoke();
    }

    public IEnumerable<InitiativeDto> GetActiveInitiatives(string tenantId)
    {
        logger.LogDebug("GetActiveInitiatives called, getting from the database");

        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;

        var data = unitOfWork.DbContext.Initiatives
            .Where(i => i.Lifetime.EndDate >= now && i.Contract!.Tenant!.Id.StartsWith(tenantId))
            .OrderBy(i => i.Code)
            .Select(InitiativeMappings.ToDto)
            .ToList();

        return data.AsReadOnly();
    }
}
