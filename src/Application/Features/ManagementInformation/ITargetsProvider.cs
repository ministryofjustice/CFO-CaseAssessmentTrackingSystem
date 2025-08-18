using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation;

public interface ITargetsProvider
{
    ContractTargetDto GetTarget(string contract, int month, int year);
    ContractTargetDto GetTargetById(string contractId, int month, int year);
}