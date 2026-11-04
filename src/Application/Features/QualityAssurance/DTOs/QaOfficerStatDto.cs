namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs;

public class QaOfficerStatDto
{
    /// Queue entry id. Notes can create multiple rows, so counts must use distinct entries.
    public Guid EntryId { get; set; }
    public string? Crn { get; set; }
    public string Queue { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? QaUserId { get; set; }
    public string? QaUser { get; set; }
    public bool IsAccepted { get; set; }
    public bool? Escalated { get; set; }
    public string? Message { get; set; }

    public string IsAcceptedText => IsAccepted ? "Yes" : "No";

    public string EscalatedText => Escalated switch
    {
        true => "Yes",
        false => "No",
        _ => string.Empty
    };

    public string Note => Message?.Replace("\r", " ").Replace("\n", " ") ?? string.Empty;
}
