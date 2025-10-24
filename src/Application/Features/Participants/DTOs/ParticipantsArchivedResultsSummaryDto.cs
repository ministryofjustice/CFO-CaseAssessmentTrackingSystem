namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantsArchivedResultsSummaryDto
{
    [Description("Participant Id")]
    public string Id { get; set; } = default!;

    [Description("Participant")]
    public string ParticipantName { get; set; } = default!;

    [Description("Status")]
    public EnrolmentStatus EnrolmentStatus { get; set; } = default!;

    [Description("Archive Date")]
    public DateTime ArchivedDate { get; set; }

    [Description("To Date")]
    public DateTime? ToDate { get; set; }

    [Description("Archive Reason")]
    public string ArchiveReason { get; set; } = default!;
       
    [Description("Archive Justification")]
    public string? ArchiveJustification { get; set; }

    [Description("Archived By")]
    public string ArchivedBy { get; set; } = default!;
}