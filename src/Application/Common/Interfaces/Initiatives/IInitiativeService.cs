using Cfo.Cats.Application.Features.Initiatives.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Initiatives;

public interface IInitiativeService
{
    IReadOnlyList<InitiativeDto> DataSource { get; }
    event Action? OnChange;
    void Refresh();
    IEnumerable<InitiativeDto> GetInitiatives(string tenantId, bool activeOnly = true);
}
