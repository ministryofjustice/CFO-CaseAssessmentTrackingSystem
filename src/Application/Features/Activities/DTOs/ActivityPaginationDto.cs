namespace Cfo.Cats.Application.Features.Activities.DTOs;

public class ActivityPaginationDto
{
    public required Guid ActivityId { get; set; }
    public required string ParticipantId { get; set; }
    public required string Participant { get; set; }
    public required ActivityStatus Status { get; set; }
    public required ActivityDefinition Definition { get; set; }
    public required DateTime? Created { get; set; }
    public required DateTime? CommencedOn { get; set; }
    public required DateTime? ApprovedOn { get; set; }
    public required DateTime? LastModified { get; set; }

    public required string SubmittedBy { get; set; }

    public ActivityQaNoteDto[] Notes { get; set; } = [];

    [Description("Additional Information")]
    public required string AdditionalInformation { get; set; }

    public required string TookPlaceAtLocationName { get; set; }

    public Guid? DocumentId { get; set; }

    public record ActSummaryNote(string Message, string CreatedBy, string TenantId, string TenantName, DateTime Created)
    {

    }
}