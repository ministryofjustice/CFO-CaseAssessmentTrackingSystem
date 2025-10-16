namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantsInLicenceEndPeriodResultsSummaryDto
{
    [Description("Participant Id")]
    public string Id { get; set; } = default!;

    [Description("Participant")]
    public string ParticipantName { get; set; } = default!;

    [Description("Status")]
    public EnrolmentStatus EnrolmentStatus { get; set; } = default!;

    public DateOnly? DeactivatedInFeed { get; set; }

    public required string TookPlaceAtLocationName { get; set; }

    public string? CaseWorkerDisplayName { get; set; }
    
    public DateOnly? PostLicenceCaseClosureEnd => DeactivatedInFeed?.AddDays(30);        
}