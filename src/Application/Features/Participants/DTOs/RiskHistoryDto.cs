namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class RiskHistoryDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public RiskReviewReason? RiskReviewReason { get; set; }
    public required DateTime CreatedDate { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime? Completed { get; set; }
    public string? CompletedBy { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int DaysSinceLastReview { get; set; }
}
