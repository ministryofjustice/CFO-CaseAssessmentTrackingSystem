using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Participants.Queries.Extensions;

public static class ParticipantFilterExtensions
{
    public static IQueryable<Participant> ApplyKeywordSearch(
        this IQueryable<Participant> query,
        string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        if (keyword.Split(" ") is { Length: 2 } segments)
        {
            return query.Where(p => p.FirstName.Contains(segments[0]) && p.LastName.Contains(segments[1]));
        }

        return query.Where(p => p.FirstName.Contains(keyword)
                             || p.LastName.Contains(keyword)
                             || p.Id.Contains(keyword)
                             || p.ExternalIdentifiers.Any(ei => ei.Value.Contains(keyword)));
    }

    public static IQueryable<Participant> ApplyLocationFilter(
        this IQueryable<Participant> query,
        int[] locations)
    {
        if (locations.Length == 0)
        {
            return query;
        }

        return query.Where(p => locations.Contains(p.CurrentLocation.Id) 
                             || (p.EnrolmentLocation != null && locations.Contains(p.EnrolmentLocation.Id)));
    }

    public static IQueryable<Participant> ApplyListViewFilter(
        this IQueryable<Participant> query,
        ParticipantListView listView) => listView switch
        {
            ParticipantListView.Default => query.Where(p =>
                p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                && p.EnrolmentStatus != EnrolmentStatus.DormantStatus.Value),
            ParticipantListView.SubmittedToAny => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value ||
                p.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus.Value),
            ParticipantListView.Identified => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.IdentifiedStatus.Value),
            ParticipantListView.Enrolling => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.EnrollingStatus.Value),
            ParticipantListView.SubmittedToProvider => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value),
            ParticipantListView.SubmittedToQa => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus.Value),
            ParticipantListView.Approved => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value),
            ParticipantListView.Dormant => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.DormantStatus.Value),
            ParticipantListView.Archived => query.Where(p =>
                p.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value),
            ParticipantListView.All => query,
            _ => throw new ArgumentOutOfRangeException(nameof(listView), listView, null)
        };

    public static IQueryable<Participant> ApplyLabelFilter(
        this IQueryable<Participant> query,
        LabelId? labelId,
        IApplicationDbContext context)
    {
        if (labelId is null)
        {
            return query;
        }

        var filteredQuery = from p in query
                           join pl in context.ParticipantLabels on p.Id equals EF.Property<string>(pl, "_participantId")
                           where EF.Property<LabelId>(pl, "LabelId") == labelId
                               && pl.Lifetime.EndDate > DateTime.UtcNow
                           select p;

        return filteredQuery.Distinct();
    }

    public static IQueryable<Participant> ApplyOwnershipFilter(
        this IQueryable<Participant> query,
        bool justMyCases,
        string? ownerId,
        string? tenantId,
        string currentUserId)
    {
        if (justMyCases)
        {
            return query.Where(p => p.OwnerId == currentUserId);
        }

        if (!string.IsNullOrEmpty(ownerId))
        {
            query = query.Where(p => p.OwnerId == ownerId);
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(p => p.Owner!.TenantId!.StartsWith(tenantId));
        }

        return query;
    }

    public static IQueryable<Participant> ApplyRiskDueFilter(
        this IQueryable<Participant> query,
        DateTime? riskDue)
    {
        if (!riskDue.HasValue)
        {
            return query;
        }

        return query.Where(p => p.RiskDue <= riskDue.Value);
    }
}
