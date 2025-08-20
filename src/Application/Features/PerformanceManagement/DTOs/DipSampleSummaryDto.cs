namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public record DipSampleSummaryDto(string ContractName, DateTime PeriodFrom, DateTime? ReviewedOn, DipSampleStatus Status)
{
    public string PeriodFromDesc => PeriodFrom.ToString("MMM yyyy");
}
