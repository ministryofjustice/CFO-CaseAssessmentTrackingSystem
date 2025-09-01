namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public record DipSampleSummaryDto(string ContractName, DateTime PeriodFrom, DateTime? ReviewedOn, DipSampleStatus Status, Guid? DocumentId)
{
    public string PeriodFromDesc => PeriodFrom.ToString("MMM yyyy");
}
