using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;
public static class GetPaidActivitiesPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<PaidActivitiesPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaidActivitiesPerSupportWorkerDto>>
    {
        public async Task<Result<PaidActivitiesPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
var context = unitOfWork.DbContext;

            var query = from mi in context.ActivityPayments
                        join ap in context.Activities on mi.ActivityId equals ap.Id
                        join l in context.Locations on mi.LocationId equals l.Id
                        where mi.EligibleForPayment &&
                              mi.ActivityApproved >= request.StartDate &&
                              mi.ActivityApproved <= request.EndDate
                        select new { mi, ap, l };

            // Checks and applies filter based on UserId or TenantId else throws exception
            query = request switch
            {
                { UserId: var userId } when !string.IsNullOrWhiteSpace(userId)
                    => query.Where(x => x.ap.OwnerId == userId),

                { TenantId: var tenantId } when !string.IsNullOrWhiteSpace(tenantId)
                    => query.Where(x => x.mi.TenantId.StartsWith(tenantId)),

                _ => throw new ArgumentException("Invalid request: UserId or TenantId must be provided.")
            };

            var groupedQuery = from x in query
                               group x.mi by new
                        {
                            x.l.Name,
                            x.l.LocationType,
                            x.ap.Type
                        } into g
                        orderby g.Key.Name, g.Key.Type
                        select new LocationDetail(
                            g.Key.Name,
                            g.Key.LocationType,
                            g.Key.Type,
                            g.Count()
                        );

            var results = await groupedQuery
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new PaidActivitiesPerSupportWorkerDto(results);
        }

    }
    public record PaidActivitiesPerSupportWorkerDto
    {
        public PaidActivitiesPerSupportWorkerDto(LocationDetail[] details)
        {
            Details = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Count);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Count);

        }
        public LocationDetail[] Details { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(string LocationName, LocationType LocationType, ActivityType ActivityType, int Count);

}