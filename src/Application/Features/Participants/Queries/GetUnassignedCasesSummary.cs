using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetUnassignedCasesSummary
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<UnassignedCasesSummaryDto>>
    {
        public required UserProfile CurrentUser { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<UnassignedCasesSummaryDto>>
    {
        public async Task<Result<UnassignedCasesSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;
            var currentTenantId = request.CurrentUser.TenantId!;

            // Get visible locations for the current tenant
            var visibleLocationIds = await (
                from l in context.Locations
                from t in l.Tenants
                where t.Id.StartsWith(currentTenantId)
                select l.Id
            ).Distinct().ToListAsync(cancellationToken);

            // Query unassigned participants grouped by location and status (for chart - includes all statuses)
            var groupedQuery = from p in context.Participants
                               where p.OwnerId == null
                                   && (visibleLocationIds.Contains(p.CurrentLocation.Id)
                                       || (p.EnrolmentLocation != null && visibleLocationIds.Contains(p.EnrolmentLocation.Id)))
                               group p by new
                               {
                                   LocationId = p.CurrentLocation.Id,  // Group by ID to avoid duplicate names
                                   LocationName = p.CurrentLocation.Name,
                                   LocationType = p.CurrentLocation.LocationType,
                                   EnrolmentStatus = p.EnrolmentStatus
                               } into grp
                               select new LocationStatusSummary
                               (
                                   grp.Key.LocationName,
                                   grp.Key.LocationType,
                                   grp.Key.EnrolmentStatus,
                                   grp.Count()
                               );

            var results = await groupedQuery
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            // Query for totals including archived (for chips)
            var totalsQuery = from p in context.Participants
                              where p.OwnerId == null
                                  && (visibleLocationIds.Contains(p.CurrentLocation.Id)
                                      || (p.EnrolmentLocation != null && visibleLocationIds.Contains(p.EnrolmentLocation.Id)))
                                  && !p.CurrentLocation.Name.Contains("Unmapped")  // Exclude unmapped from main totals
                              group p by p.CurrentLocation.LocationType into grp
                              select new { LocationType = grp.Key, Count = grp.Count() };

            var totals = await totalsQuery
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            // Also get counts for unmapped locations (for chips)
            var unmappedCustodyCount = await context.Participants
                .Where(p => p.OwnerId == null
                           && (visibleLocationIds.Contains(p.CurrentLocation.Id)
                               || (p.EnrolmentLocation != null && visibleLocationIds.Contains(p.EnrolmentLocation.Id)))
                           && p.CurrentLocation.Name.Contains("Unmapped Custody"))
                .CountAsync(cancellationToken);

            var unmappedCommunityCount = await context.Participants
                .Where(p => p.OwnerId == null
                           && (visibleLocationIds.Contains(p.CurrentLocation.Id)
                               || (p.EnrolmentLocation != null && visibleLocationIds.Contains(p.EnrolmentLocation.Id)))
                           && p.CurrentLocation.Name.Contains("Unmapped Community"))
                .CountAsync(cancellationToken);

            var custodyTotal = totals.Where(t => t.LocationType.IsCustody).Sum(t => t.Count);
            var communityTotal = totals.Where(t => t.LocationType.IsCommunity).Sum(t => t.Count);

            return Result<UnassignedCasesSummaryDto>.Success(
                new UnassignedCasesSummaryDto(results, custodyTotal, communityTotal, unmappedCustodyCount, unmappedCommunityCount));
        }
    }

    public record UnassignedCasesSummaryDto
    {
        public UnassignedCasesSummaryDto(LocationStatusSummary[] summaries, int custodyTotal, int communityTotal, 
            int unmappedCustodyTotal, int unmappedCommunityTotal)
        {
            Summaries = summaries;
            TotalCount = summaries.Sum(s => s.Count);
            Custody = custodyTotal;
            Community = communityTotal;
            UnmappedCustody = unmappedCustodyTotal;
            UnmappedCommunity = unmappedCommunityTotal;
        }

        public LocationStatusSummary[] Summaries { get; }
        public int TotalCount { get; }
        public int Custody { get; }
        public int Community { get; }
        public int UnmappedCustody { get; }
        public int UnmappedCommunity { get; }
    }

    public record LocationStatusSummary(
        string LocationName,
        LocationType LocationType,
        EnrolmentStatus EnrolmentStatus,
        int Count
    );
}
