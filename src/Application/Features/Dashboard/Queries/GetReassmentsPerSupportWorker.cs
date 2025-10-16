using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetReassessmentsPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ReassessmentsPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string UserId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ReassessmentsPerSupportWorkerDto>>
    {
        public async Task<Result<ReassessmentsPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from mi in context.ReassessmentPayments
                        join l in context.Locations on mi.LocationId equals l.Id
                        where mi.SupportWorker == request.UserId
                        && mi.AssessmentCompleted >= request.StartDate
                        && mi.AssessmentCompleted <= request.EndDate
                        group mi by l into grp
                        select new Details
                            (
                                grp.Key.Name,
                                grp.Key.LocationType,
                                grp.Count(mi => mi.EligibleForPayment),
                                grp.Count()
                            );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new ReassessmentsPerSupportWorkerDto(results);
        }
    }

    public record ReassessmentsPerSupportWorkerDto
    {
        public ReassessmentsPerSupportWorkerDto(Details[] details)
        {
            Details = details;
            
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.TotalCount);
            CustodyPayable = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Payable);

            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.TotalCount);
            CommunityPayable = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Payable);
        }

        public Details[] Details { get; }

        public int Custody { get; }
        public int CustodyPayable { get; }

        public int Community { get; }
        public int CommunityPayable { get; }

    }

    public record Details(string LocationName, LocationType LocationType, int Payable, int TotalCount);

}
