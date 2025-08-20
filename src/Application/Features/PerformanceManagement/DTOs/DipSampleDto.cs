namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

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
    int TotalReviewed = 0,
    int TotalVerified = 0,
    int TotalFinalised = 0,
    DateTime? CompletedOn = null, 
    string? CompletedBy = null,
    string[]? Reviewers = null,
    string[]? Verifiers  = null)
{
    public int? Score => FinalScore ?? CpmScore ?? CsoScore;
}