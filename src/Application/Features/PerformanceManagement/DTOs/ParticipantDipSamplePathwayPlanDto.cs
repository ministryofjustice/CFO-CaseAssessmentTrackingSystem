namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSamplePathwayPlanDto
{
    public required string ParticipantId { get; init; }
    public required ParticipantDipSampleObjectiveDto[] Objectives { get; init; }
}