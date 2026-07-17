using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Queries.Extensions;

public static class ParticipantRecentActionExtensions
{
    public static IQueryable<Participant> ApplyRecentActionFilter(
        this IQueryable<Participant> query,
        RecentParticipantFilter recentAction,
        string currentUserId,
        IApplicationDbContext context)
    {
        var recentlyAssignedCutoff = recentAction switch
        {
            RecentParticipantFilter.AssignedLast10Days => DateTime.UtcNow.Date.AddDays(-10),
            RecentParticipantFilter.AssignedLast30Days => DateTime.UtcNow.Date.AddDays(-30),
            _ => (DateTime?)null
        };

        var recentlyVisitedCutoff = recentAction switch
        {
            RecentParticipantFilter.VisitedLast7Days => DateTime.UtcNow.Date.AddDays(-7),
            _ => (DateTime?)null
        };

        var recentlyArchivedCutoff = recentAction switch
        {
            RecentParticipantFilter.ArchivedLast30Days => DateTime.UtcNow.Date.AddDays(-30),
            _ => (DateTime?)null
        };

        var deactivatedInFeedCutoff = recentAction switch
        {
            RecentParticipantFilter.LicenceEndPeriod => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            _ => (DateOnly?)null
        };

        if (deactivatedInFeedCutoff.HasValue)
        {
            query = query.Where(p => p.DeactivatedInFeed >= deactivatedInFeedCutoff.Value);
        }

        if (recentlyArchivedCutoff.HasValue)
        {
            var participantIdsArchivedRecently = context.ParticipantEnrolmentHistories
                .Where(peh => peh.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value
                              && peh.From >= recentlyArchivedCutoff.Value)
                .Select(peh => peh.ParticipantId)
                .Distinct();

            query = query.Where(p => participantIdsArchivedRecently.Contains(p.Id));
        }

        if (recentlyAssignedCutoff.HasValue)
        {
            var participantIdsWithRecentOwnership = context.ParticipantOwnershipHistories
                .Where(oh => oh.OwnerId == currentUserId
                             && oh.From >= recentlyAssignedCutoff.Value
                             && oh.To == null)
                .Select(oh => oh.ParticipantId)
                .Distinct();

            query = query.Where(p => participantIdsWithRecentOwnership.Contains(p.Id));
        }

        if (recentlyVisitedCutoff.HasValue)
        {
            var participantIdsWithAccessAuditTrail = context.AccessAuditTrails
                .Where(oh => oh.UserId == currentUserId
                             && oh.AccessDate >= recentlyVisitedCutoff.Value)
                .Select(oh => oh.ParticipantId)
                .Distinct();

            query = query.Where(p => participantIdsWithAccessAuditTrail.Contains(p.Id));
        }

        return query;
    }
}
