namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record DipSampleDto(
    Guid Id,
    string ContractDescription, 
    DipSampleStatus Status,
    DateTime PeriodFrom,
    DateTime CreatedOn,
    int Size,
    double? ScoreAvg = null,
    DateTime? CompletedOn = null, 
    string? CompletedBy = null);