namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantLocationHistoryDto
{
    public required int Id { get; init; }

    [Description("Date")]
    public required DateTime? MoveDate { get; init; }

    [Description("Performed By")]
    public required string SupportWorker { get; set; }

    public string? SupportWorkerTenantId { get; set; }

    [Description("Location")]
    public required string LocationName { get; init; }
    
    [Description("Days")]
    public int DaysSincePrevious { get; set; }

    public bool IsEnrolment { get; set; }
}