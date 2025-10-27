namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public class MyTeamsParticipantsWithNoRiskDto
{
    public required string ParticipantId { get; set; }

    public required string ParticipantName { get; set; }
    public required DateTime? CaseCreatedDate { get; set; }
    public required EnrolmentStatus EnrolmentStatus { get; set; }
}