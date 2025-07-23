namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSampleIswActivityDto : ParticipantDipSampleActivityDto
{
    public DateTime WraparoundSupportStartedOn { get; init; }
    public double HoursPerformedPre { get; init; }
    public double HoursPerformedDuring { get; init; }
    public double HoursPerformedPost { get; init; }
    public DateTime BaselineAchievedOn { get; init; }
    public required Guid DocumentId { get; init; }

    public override bool HasTemplate() => true;
}