namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public class RecentlyApprovedActivitiesSummaryDto
{
    public required string ParticipantId { get; set; }
    public required string Participant { get; set; }
    public required ActivityDefinition Definition { get; set; }
    public required DateTime? ApprovedOn { get; set; }
}
