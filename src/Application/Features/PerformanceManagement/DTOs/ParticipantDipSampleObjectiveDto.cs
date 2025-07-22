namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSampleObjectiveDto
{
    public DateTime Created { get; init; }
    public required string CreatedBy { get; init; }
    public required string Description { get; init; }
    
    public string? Status { get; init; }
    public string? Justification { get; init; }
    public DateTime? Completed { get; init; }
    public string? CompletedBy { get; init; }
    
    public required int Index { get; init; }

    public ParticipantDipSampleObjectiveTaskDto[] Tasks { get; init; } = [];
}