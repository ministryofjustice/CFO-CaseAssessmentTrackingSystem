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
        public required string UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaidActivitiesPerSupportWorkerDto>>
    {
        public async Task<Result<PaidActivitiesPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from mi in context.ActivityPayments
                        join a in context.Activities on mi.ActivityId equals a.Id
                        join l in context.Locations on mi.LocationId equals l.Id
                        where mi.EligibleForPayment
                              && a.OwnerId == request.UserId
                              && mi.ActivityApproved >= request.StartDate
                              && mi.ActivityApproved <= request.EndDate
                        group mi by new
                        {
                            l.Name,
                            l.LocationType,
                            mi.ActivityType
                        } into g
                        orderby g.Key.Name, g.Key.ActivityType
                        select new LocationDetail(
                            g.Key.Name,
                            g.Key.LocationType,
                            g.Key.ActivityType,
                            g.Count()
                        );

            var results = await query
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
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.TotalCount);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.TotalCount);

        }
        public LocationDetail[] Details { get; }
        public int Custody { get; }
        public int Community { get; }
    }

    public record LocationDetail(string Name, LocationType LocationType, string ActivityType, int TotalCount);

}