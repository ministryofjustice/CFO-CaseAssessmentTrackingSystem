using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface ICumulativeExcelService
{
    ICumulativeExcelService WithThisMonth(DateOnly date);
    ICumulativeExcelService WithActuals(Actuals actuals);
    ICumulativeExcelService WithTargets(ContractTargetDto targets);
    ICumulativeExcelService WithLastMonthActuals(Actuals actuals);
    ICumulativeExcelService WithLastMonthTargets(ContractTargetDto targets);
    Task<byte[]> ExportAsync();
}
