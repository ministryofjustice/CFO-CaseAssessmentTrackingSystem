using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using static Cfo.Cats.Application.Features.Dashboard.Queries.GetActivitiesPerSupportWorker.Handler;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;
public static class GetActivitiesPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ActivitiesPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ActivitiesPerSupportWorkerDto>>
    {
        public async Task<Result<ActivitiesPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from mi in context.ActivityPayments
                        join a in context.Activities on mi.ActivityId equals a.Id
                        join l in context.Locations on mi.LocationId equals l.Id
                        where a.OwnerId == request.UserId
                        && mi.ActivityApproved >= request.StartDate
                        && mi.ActivityApproved <= request.EndDate
                        group mi by new
                        {
                            LocationId = l.Id,
                            LocationName = l.Name,
                            LocationType = l.LocationType,
                            ActivityType = mi.ActivityType
                        } into grp

                        select new LocationDetail
                            (
                                grp.Key.LocationName,
                                grp.Key.LocationType,
                                grp.Key.ActivityType,
                                grp.Count(mi => mi.EligibleForPayment),
                                grp.Count()
                            );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new ActivitiesPerSupportWorkerDto(results);
        }
    }
    public record ActivitiesPerSupportWorkerDto
    {
        public ActivitiesPerSupportWorkerDto(LocationDetail[] details)
        {
            Details = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.TotalCount);
            CustodyPayable = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Payable);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.TotalCount);
            CommunityPayable = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Payable);
        }
        public LocationDetail[] Details { get; }
        public int Custody { get; }
        public int CustodyPayable { get; }
        public int Community { get; }
        public int CommunityPayable { get; }
    }

    public record LocationDetail(string Name, LocationType LocationType, string ActivityType, int Payable, int TotalCount);

    
}