using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetCasesPerLocationBySupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<CasesPerLocationSupportWorkerDto>>
    {
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<CasesPerLocationSupportWorkerDto>>
    {
        public async Task<Result<CasesPerLocationSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from p in context.Participants
                        join l in context.Locations on p.CurrentLocation!.Id equals l.Id
                        join u in context.Users on p.OwnerId equals u.Id
                        where p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                        select new { p, l, u };

            // Checks and applies filter based on UserId or TenantId else throws exception
            query = request switch
            {
                { UserId: var userId } when !string.IsNullOrWhiteSpace(userId)
                    => query.Where(x => x.p.OwnerId == userId),

                { TenantId: var tenantId } when !string.IsNullOrWhiteSpace(tenantId)
                    => query.Where(x => x.u.TenantId!.StartsWith(tenantId)),

                _ => throw new ArgumentException("Invalid request: UserId or TenantId must be provided.")
            };

            var groupedQuery = from x in query
                               group x.p by new
                {
                    x.l.Name,
                    x.l.LocationType,
                    x.p.EnrolmentStatus
                } into grp
                select new LocationDetail
                (
                    grp.Key.Name,
                    grp.Key.LocationType,
                    grp.Key.EnrolmentStatus,
                    grp.Count()
                );

            var results = await groupedQuery
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new CasesPerLocationSupportWorkerDto(results);
        }        
    }

    public record CasesPerLocationSupportWorkerDto
    {
        public CasesPerLocationSupportWorkerDto(LocationDetail[] details)
        {
            Records = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Count);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Count);
        }
        public LocationDetail[] Records { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(string LocationName, LocationType LocationType, EnrolmentStatus Status, int Count);
}