namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSampleEmploymentActivityDto : ParticipantDipSampleActivityDto
{
    public required string EmploymentType { get; init; }
    public required string EmployerName { get; init; }
    public required string JobTitle { get; init; }
    public required string JobTitleCode { get; init; } 
    public double? Salary { get; init; }
    public string? SalaryFrequency { get; init; }
    public DateTime EmploymentCommenced { get; init; }
    public Guid DocumentId { get; init; }
}