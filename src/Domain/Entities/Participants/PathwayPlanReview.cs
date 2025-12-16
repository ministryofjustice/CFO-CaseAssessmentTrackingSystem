using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class PathwayPlanReview : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PathwayPlanReview()
    {
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static PathwayPlanReview Create(Guid pathwayPlanId, string participantId, int locationId,
        DateTime reviewDate, string? review, PathwayPlanReviewReason reviewReason)
    {
        PathwayPlanReview pathwayPlanReview = new()
        {
            PathwayPlanId = pathwayPlanId,
            ParticipantId = participantId,
            LocationId = locationId,
            ReviewDate = reviewDate,
            Review = review,
            ReviewReason = reviewReason
        };

        pathwayPlanReview.AddDomainEvent(new PathwayPlanReviewAddedDomainEvent(pathwayPlanReview));

        return pathwayPlanReview;
    }

    public Guid PathwayPlanId { get; private set; }
    public int LocationId { get; private set; }
    public DateTime ReviewDate { get; private set; }
    public string? Review { get; private set; }
    public PathwayPlanReviewReason ReviewReason { get; private set; }
    public string ParticipantId { get; private set; }
}