namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantEnrolmentHistoryDto
{
    public required int Id { get; init; }

    [Description("Date")]
    public required DateOnly? ActionDate { get; init; }

    [Description("Status")]
    public required string Status { get; init; }

    [Description("Event")]
    public required string Event { get; init; }

    [Description("Reason")]
    public string? Reason { get; set; }

    [Description("Additional Information")]
    public required string AdditionalInformation { get; init; }

    [Description("Performed By")]
    public required string SupportWorker { get; set; }

    public string? SupportWorkerTenantId { get; set; }

    [Description("Location")]
    public required string LocationName { get; init; }

    [Description("Days")]
    public int DaysSincePrevious { get; set; } 
}