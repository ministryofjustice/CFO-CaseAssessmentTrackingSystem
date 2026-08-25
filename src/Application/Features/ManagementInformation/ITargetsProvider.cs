using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation;

public interface ITargetsProvider
{
    IReadOnlyList<ContractTargetDto> DataSet { get; }

    ContractTargetDto GetTarget(string contract, int month, int year);
    ContractTargetDto GetTargetById(string contractId, int month, int year);

    /// <summary>
    /// Force the provider to refresh, bypassing / clearing any cached data.
    /// </summary>
    void Refresh();
}