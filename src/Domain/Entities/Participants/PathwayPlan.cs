using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class PathwayPlan : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PathwayPlan()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private readonly List<Objective> _objectives =  new();
    
    public string ParticipantId { get; private set; }

    public IReadOnlyList<Objective> Objectives => _objectives;
    
    private readonly List<PathwayPlanReview> _pathwayPlanReviews = new();
    public IReadOnlyCollection<PathwayPlanReview> PathwayPlanReviews => _pathwayPlanReviews.AsReadOnly();

    public void AddObjective(Objective objective) => _objectives.Add(objective.AtIndex(_objectives.Count + 1));

    public void Review(Guid pathwayPlanId, string participantId,  int locationId, DateTime reviewDate, string? review, PathwayPlanReviewReason reviewReason)
    {
        var pathwayPlanReview = PathwayPlanReview.Create(pathwayPlanId,participantId,locationId,reviewDate,review,reviewReason);
        _pathwayPlanReviews.Add(pathwayPlanReview);
    }

    public static PathwayPlan Create(string participantId)
    {
        PathwayPlan pathwayPlan = new()
        {
            ParticipantId = participantId
        };

        pathwayPlan.AddDomainEvent(new PathwayPlanCreatedDomainEvent(pathwayPlan));

        return pathwayPlan;
    }
}