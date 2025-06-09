using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation;

public interface ITargetsProvider
{
    public ContractTargetDto GetTarget(string contract, int month, int year);
    public ContractTargetDto GetTargetById(string contractId, int month, int year);
}