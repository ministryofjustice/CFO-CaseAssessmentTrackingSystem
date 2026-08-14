using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Features.PRIs.Queries.Extensions;

public static class PRIFilterExtensions
{
    public static IQueryable<PRI> ApplyTenantFilter(
        this IQueryable<PRI> query,
        UserProfile currentUser)
        // All authorized users can see PRIs in their tenant hierarchy
        // This matches the pattern used in ParticipantsWithPagination
        => query.Where(x => x.Participant!.Owner!.TenantId!.StartsWith(currentUser.TenantId!));

    public static IQueryable<PRI> ApplyUserFilter(
        this IQueryable<PRI> query,
        bool includeOutgoing,
        bool includeIncoming,
        string userId,
        string? custodyWorkerFilter = null,
        string? communityWorkerFilter = null)
    {
        // If both are true or both are false, show all (no additional filtering needed)
        if (includeOutgoing == includeIncoming)
        {
            return query;
        }

        // Skip user filter for outgoing if a specific custody worker is selected
        // (they would conflict on CreatedBy)
        if (includeOutgoing && string.IsNullOrWhiteSpace(custodyWorkerFilter))
        {
            return query.Where(p => p.CreatedBy == userId);
        }

        // Skip user filter for incoming if a specific community worker is selected
        // (they would conflict on AssignedTo)
        if (includeIncoming && string.IsNullOrWhiteSpace(communityWorkerFilter))
        {
            return query.Where(p => p.AssignedTo == userId);
        }

        return query;
    }

    public static IQueryable<PRI> ApplyKeywordSearch(
        this IQueryable<PRI> query,
        string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        return query.Where(p => 
            p.ParticipantId!.Contains(keyword)
            || p.Participant!.FirstName.Contains(keyword)
            || p.Participant!.LastName.Contains(keyword)
            || p.ExpectedReleaseRegion.Name.Contains(keyword));
    }

    public static IQueryable<PRI> ApplyCustodyWorkerFilter(
        this IQueryable<PRI> query,
        string? custodyWorkerId)
    {
        if (string.IsNullOrWhiteSpace(custodyWorkerId))
        {
            return query;
        }

        return query.Where(p => p.CreatedBy == custodyWorkerId);
    }

    public static IQueryable<PRI> ApplyCommunityWorkerFilter(
        this IQueryable<PRI> query,
        string? communityWorkerId)
    {
        if (string.IsNullOrWhiteSpace(communityWorkerId))
        {
            return query;
        }

        return query.Where(p => p.AssignedTo == communityWorkerId);
    }

    public static IQueryable<PRI> ApplyRegionFilter(
        this IQueryable<PRI> query,
        int? regionId)
    {
        if (!regionId.HasValue)
        {
            return query;
        }

        return query.Where(p => p.ExpectedReleaseRegionId == regionId.Value);
    }

    public static IQueryable<PRI> ApplyActiveStatusFilter(
        this IQueryable<PRI> query,
        bool? activeStatus)
    {
        if (!activeStatus.HasValue)
        {
            return query;
        }

        if (activeStatus.Value)
        {
            // Active participants - use EnrolmentStatus.ActiveList which EF Core can translate
            return query.Where(p => EnrolmentStatus.ActiveList.Contains(p.Participant!.EnrolmentStatus));
        }
        else
        {
            // Inactive participants - NOT in ActiveList
            return query.Where(p => !EnrolmentStatus.ActiveList.Contains(p.Participant!.EnrolmentStatus));
        }
    }
}
