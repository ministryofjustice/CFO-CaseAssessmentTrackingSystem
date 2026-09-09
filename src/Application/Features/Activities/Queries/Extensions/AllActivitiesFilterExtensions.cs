using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries.Extensions;

/// <summary>
/// Filters shared between <see cref="AllActivitiesWithPagination"/> and <see cref="GetActivityAssignees"/>
/// that cannot be expressed as part of <see cref="Specifications.AllActivitiesAdvancedSpecification"/>
/// because they need access to other DbSets on the context.
/// </summary>
public static class AllActivitiesFilterExtensions
{
    public static IQueryable<Activity> ApplyReturnedWithinDaysFilter(
        this IQueryable<Activity> query,
        IApplicationDbContext db,
        int? returnedWithinDays)
    {
        if (returnedWithinDays is not { } days)
        {
            return query;
        }

        var cutoff = DateTime.UtcNow.AddDays(-days);

        // Note: we ignore QA1 returns because these are not returned to the provider
        var returnedActivityIds =
            db.ActivityPqaQueue // PQA returns
                .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                .Select(e => e.ActivityId)
            .Union(db.ActivityQa2Queue // QA2 returns
                .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                .Select(e => e.ActivityId))
            .Union(db.ActivityEscalationQueue // Escalation returns
                .Where(e => e.IsCompleted && e.IsAccepted == false && e.LastModified >= cutoff)
                .Select(e => e.ActivityId));

        return query.Where(a => returnedActivityIds.Contains(a.Id));
    }

    public static IQueryable<Activity> ApplyApprovedWithinDaysFilter(
        this IQueryable<Activity> query,
        int? approvedWithinDays)
    {
        if (approvedWithinDays is not { } days)
        {
            return query;
        }

        var approvedCutoff = DateTime.UtcNow.AddDays(-days).Date;

        return query.Where(a => a.Status == ActivityStatus.ApprovedStatus.Value
                                 && a.CompletedOn >= approvedCutoff);
    }
}
