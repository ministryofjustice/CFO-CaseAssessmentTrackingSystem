namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs;

public class QAEnrolmentsResultsSummaryDto
{
    public required string ParticipantId { get; set; }
    public required string Participant { get; set; }
    public required EnrolmentStatus Status { get; set; }
    public required EnrolmentSummaryNote[] PQA { get; set; }
    public required EnrolmentSummaryNote[] QA1 { get; set; }
    public required EnrolmentSummaryNote[] QA2 { get; set; }
    public required EnrolmentSummaryNote[] Escalations { get; set; }
    public required DateTime? Created { get; set; }
    public required DateTime? CommencedOn { get; set; }
    public required DateTime? ApprovedOn { get; set; }
    public required DateTime? LastModified { get; set; }

    public required string SubmittedBy { get; set; }

    public EnrolmentQaNoteDto[] Notes { get; set; } = [];

    //[Description("Additional Information")]
    //public required string AdditionalInformation { get; set; }

    //public required string TookPlaceAtLocationName { get; set; }

    public EnrolmentSummaryNote[] GetNotes()
    {
        return (PQA.Union(QA1).Union(QA2).Union(Escalations)).OrderBy(x => x.Created).ToArray();
    }

    public record EnrolmentSummaryNote(string Message, string CreatedBy, string TenantId, string TenantName, DateTime Created)
    {

    }
}