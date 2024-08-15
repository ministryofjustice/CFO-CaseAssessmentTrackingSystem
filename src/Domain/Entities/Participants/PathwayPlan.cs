using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class PathwayPlan : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PathwayPlan()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private List<Objective> _objectives = new();
    private List<PathwayPlanReviewHistory> _reviewHistories = new();

    public string ParticipantId { get; private set; }

    public IReadOnlyList<Objective> Objectives => _objectives;
    public IReadOnlyList<PathwayPlanReviewHistory> ReviewHistories => _reviewHistories;

    public void AddObjective(Objective objective)
    {
        _objectives.Add(objective);
    }

    public void Review()
    {
        var history = PathwayPlanReviewHistory.Create(Id);
        _reviewHistories.Add(history);
    }

    public static PathwayPlan Create(string participantId)
    {
        PathwayPlan pathwayPlan = new()
        {
            ParticipantId = participantId
        };

        return pathwayPlan;
    }

}
