using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class PathwayPlanReviewHistory : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PathwayPlanReviewHistory()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Guid PathwayPlanId { get; private set; }

    public static PathwayPlanReviewHistory Create(Guid pathwayPlanId)
    {
        PathwayPlanReviewHistory history = new()
        {
            PathwayPlanId = pathwayPlanId
        };

        return history;
    }

}
