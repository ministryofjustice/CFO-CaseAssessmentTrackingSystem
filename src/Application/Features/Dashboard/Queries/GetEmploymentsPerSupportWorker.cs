using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;
public static class GetEmploymentsPerSupportWorker
{ 
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<EmploymentsPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<EmploymentsPerSupportWorkerDto>>
    {
        public async Task<Result<EmploymentsPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from mi in context.EmploymentPayments
                        join ap in context.Activities on mi.ActivityId equals ap.Id
                        join l in context.Locations on mi.LocationId equals l.Id
                        where ap.OwnerId == request.UserId
                        && mi.ActivityApproved >= request.StartDate
                        && mi.ActivityApproved <= request.EndDate
                        group mi by l into grp
                        orderby grp.Key.Name, grp.Key.LocationType
                        select new LocationDetail
                            (
                                grp.Key.Name,
                                grp.Key.LocationType,
                                grp.Count(mi => mi.EligibleForPayment),
                                grp.Count()
                            );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new EmploymentsPerSupportWorkerDto(results);
        }
    }

    public record EmploymentsPerSupportWorkerDto
    {
        public EmploymentsPerSupportWorkerDto(LocationDetail[] details)
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

    public record LocationDetail(string Name, LocationType LocationType, int Payable, int TotalCount);

}