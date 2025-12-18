namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class PathwayPlanReviewHistoryDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; }
    public string? Review { get; set; }
    public PathwayPlanReviewReason? ReviewReason { get; set; }
    public required DateTime ReviewDate { get; set; }
    public required string ReviewedBy { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int DaysSinceLastReview { get; set; }
}