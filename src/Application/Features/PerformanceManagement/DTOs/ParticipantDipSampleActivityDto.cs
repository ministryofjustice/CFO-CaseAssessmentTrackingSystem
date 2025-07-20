namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public abstract class ParticipantDipSampleActivityDto
{
    public Guid TaskId { get; set; }
    public DateTime? ApprovedOn { get; init; }
    public string? AdditionalInformation { get; init; }
    public DateTime? CommencedOn { get; init; }
    public DateTime Created { get; init; }
    public required string CreatedBy { get; init; }
    public required ActivityCategory Category { get; init; }
    public required ActivityDefinition Definition { get; init; }
    public required ActivityStatus Status { get; init; }
    public required ActivityType Type { get; init; }
    public required string Location { get; init; }

}