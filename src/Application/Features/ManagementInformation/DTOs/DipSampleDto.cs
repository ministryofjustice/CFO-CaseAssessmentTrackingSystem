namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record DipSampleDto(
    Guid Id,
    string ContractDescription, 
    DipSampleStatus Status,
    DateTime PeriodFrom,
    DateTime CreatedOn,
    int Size,
    int? CsoScore = null,
    int? CpmScore = null,
    int? FinalScore = null,
    int? CsoPercentage = null,
    int? CpmPercentage = null,
    int? FinalPercentage = null,
    int TotalScored = 0,
    DateTime? CompletedOn = null, 
    string? CompletedBy = null)
{
    public int? Score => FinalScore ?? CpmScore ?? CsoScore;
}