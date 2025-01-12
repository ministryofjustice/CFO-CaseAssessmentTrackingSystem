using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces.Contracts;

public interface IContractService
{
    List<ContractDto> DataSource { get; }
    event Action? OnChange;
    void Initialize();
    void Refresh();
    IEnumerable<ContractDto> GetVisibleContracts(string tenantId);
}