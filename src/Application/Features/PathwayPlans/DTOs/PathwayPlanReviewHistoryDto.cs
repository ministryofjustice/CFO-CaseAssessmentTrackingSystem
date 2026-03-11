namespace Cfo.Cats.Application.Features.PathwayPlans.DTOs;

public class PathwayPlanReviewHistoryDto
{
    public required Guid Id { get; init; }
    public required string ParticipantId { get; set; }
    public string? Review { get; init; }
    public PathwayPlanReviewReason? ReviewReason { get; init; }
    public required DateTime ReviewDate { get; init; }
    public required string ReviewedBy { get; init; }
    public int LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;
    public required DateTime Created { get; init; }
    public int? DaysSinceLastReview { get; init; }
    public bool IsEditable { get; init; }
}