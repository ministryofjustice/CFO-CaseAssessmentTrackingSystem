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

            // Base query: unassigned participants at accessible locations without inaccessible incoming transfers
            var baseQuery = from p in context.Participants
                            where p.OwnerId == null
                                && p.CurrentLocation.Tenants.Any(t => t.Id.StartsWith(currentTenantId))
                                && (!context.ParticipantIncomingTransferQueue.Any(t => 
                                    t.ParticipantId == p.Id 
                                    && !t.Completed 
                                    && !t.ToLocation.Tenants.Any(lt => lt.Id.StartsWith(currentTenantId))))
                            select p;

            // Group by location and status for chart data
            var groupedQuery = from p in baseQuery
                               group p by new
                               {
                                   LocationId = p.CurrentLocation.Id,
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

            // Group by location type for chip totals
            var totalsQuery = from p in baseQuery
                              group p by p.CurrentLocation.LocationType into grp
                              select new { LocationType = grp.Key, Count = grp.Count() };

            var totals = await totalsQuery
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            var custodyTotal = totals.Where(t => t.LocationType.IsCustody).Sum(t => t.Count);
            var communityTotal = totals.Where(t => t.LocationType.IsCommunity).Sum(t => t.Count);

            return Result<UnassignedCasesSummaryDto>.Success(
                new UnassignedCasesSummaryDto(results, custodyTotal, communityTotal));
        }
    }

    public record UnassignedCasesSummaryDto
    {
        public UnassignedCasesSummaryDto(LocationStatusSummary[] summaries, int custodyTotal, int communityTotal)
        {
            Summaries = summaries;
            TotalCount = summaries.Sum(s => s.Count);
            Custody = custodyTotal;
            Community = communityTotal;
        }

        public LocationStatusSummary[] Summaries { get; }
        public int TotalCount { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationStatusSummary(
        string LocationName,
        LocationType LocationType,
        EnrolmentStatus EnrolmentStatus,
        int Count
    );
}
