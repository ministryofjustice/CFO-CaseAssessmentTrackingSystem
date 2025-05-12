using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Contracts;

public interface IContractService
{
    IReadOnlyList<ContractDto> DataSource { get; }
    event Action? OnChange;
    void Refresh();
    IEnumerable<ContractDto> GetVisibleContracts(string tenantId);
}