namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class QAActivitiesResultsSummaryDto
{
    public required Guid ActivityId { get; set; }
    public required string ParticipantId { get; set; }
    public required string Participant { get; set; }
    public required ActivityStatus Status { get; set; }
    public required ActivityDefinition Definition { get; set; }
    public required ActSummaryNote[] PQA { get; set; }
    public required ActSummaryNote[] QA1 { get; set; }
    public required ActSummaryNote[] QA2 { get; set; }
    public required ActSummaryNote[] Escalations { get; set; }
   // public required Guid TaskId { get; set; }
    public required DateTime? Created { get; set; }
    public required DateTime? CommencedOn { get; set; }
    public required DateTime Expiry { get; set; }
    public required DateTime? ApprovedOn { get; set; }
    public required DateTime? LastModified { get; set; }

    public ActivityQaNoteDto[] Notes { get; set; } = [];

    [Description("Additional Information")]
    public required string AdditionalInformation { get; set; }

    public required string TookPlaceAtLocationName { get; set; }

    public ActSummaryNote[] GetNotes()
    {
        return (PQA.Union(QA1).Union(QA2).Union(Escalations)).OrderBy(x => x.Created).ToArray();
    }

    public record ActSummaryNote(string Message, string CreatedBy, string TenantId, string TenantName, DateTime Created)
    {

    }
}